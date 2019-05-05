using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Cinemateque.Data
{
    public class FilmService : IFilmService
    {
        private readonly CinematequeContext _context;

        public CinematequeContext Context => _context;

        public FilmService(CinematequeContext context)
        {
            _context = context;
        }

        public IEnumerable<Film> SearchFilms(string name, string genre, string director, string actor)
        {
            IEnumerable<Film> res = GetFilms().Where(f => f.FilmName.Contains(name ?? "")
                                                             && f.Genre.Contains(genre ?? "")
                                                             && f.Director.DirectorName.Contains(director ?? "")
                                                             && f.FilmActors.Any(a =>
                                                                 a.Actor.ActorName.Contains(actor ?? "")));
            return res == null ? null : res.ToArray();
        }

        public IQueryable<Film> GetFilms()
        {
            return _context.Film.Include(f => f.Director)
                .Include(f => f.FilmActors)
                .Include(f => f.FilmReward)
                .Include(f => f.UserFilms);
        }

        public IQueryable<UserFilms> GetUserFilms()
        {
            return _context.UserFilms.Include(u => u.Film)
                .Include( u => u.Film.Director )
                .Include(u => u.User);
        }

        public IQueryable<FilmActors> GetFilmActors()
        {
            return _context.FilmActors.Include(f => f.Film)
                .Include(f => f.Actor);
        }

        public IQueryable<FilmReward> GetFilmRewards()
        {
            return _context.FilmReward.Include(f => f.Film);             
        }

        public IQueryable<User> GetUser()
        {
            return _context.User.Include( u => u.UserFilms);
        }

        public IEnumerable<Film> GetHistory(int userId)
        {
            var history = GetUserFilms().Where(f => f.UserId == userId && f.Status == nameof(FilmStatus.seen)).Select(f => f.Film).ToArray();
            return history;
        }

        public IEnumerable<Film> GetWatchLater(int userId)
        {
            var history = GetUserFilms().Where(f => f.UserId == userId && f.Status == nameof(FilmStatus.later)).Select(f => f.Film).ToArray();
            return history;
        }

        public async Task<Film> AddFilm(Film film)
        {
            var addedFilm = _context.Film.Add(film);
            await _context.SaveChangesAsync();
            return addedFilm.Entity;
        }

        public async Task<FilmReward> AddAwards(FilmReward film)
        {
            var addedFilm = _context.FilmReward.Add(film);
            await _context.SaveChangesAsync();
            return addedFilm.Entity;
        }

        public async Task<FilmActors> AddFilmActor(FilmActors film)
        {
            var addedFilm = _context.FilmActors.Add(film);
            await _context.SaveChangesAsync();
            return addedFilm.Entity;
        }

        public Film GetSuggestion(string username)
        {
            var user = GetUser().Where(u => u.UserName == username);
            var userFilms = GetUserFilms().Where(f => f.User.UserName == username).Select(f => f.Film);

            var favoriteGenre = userFilms.GroupBy(f => f.Genre)
                .OrderByDescending(g => g.Count()).First().Select(f => f.Genre).First();

            var favoriteDirector = userFilms.GroupBy(f => f.Director.DirectorName)
                .OrderByDescending(g => g.Count()).First().Select(f => f.Director.DirectorName).First();

            var favoriteActor = userFilms.SelectMany(f => f.FilmActors)
                .GroupBy(a => a.Actor.ActorName).OrderByDescending(a => a.Count()).First()
                .Select(f => f.Actor.ActorName).First();

            var interestingFilms = SearchFilms(null, favoriteGenre, favoriteDirector, favoriteActor);
            var filmsList = interestingFilms.Except(userFilms).ToList();

            Random rd = new Random();

            if (filmsList != null || filmsList.Count() == 0)
            {
                var random = rd.Next(0, filmsList.Count());
                return filmsList[random];
            }

            interestingFilms = SearchFilms(null, favoriteGenre, favoriteDirector, null);
            filmsList = interestingFilms.Except(userFilms).ToList();

            if (filmsList != null || filmsList.Count() == 0)
            {
                var random = rd.Next(0, filmsList.Count());
                return filmsList[random];
            }

            interestingFilms = SearchFilms(null, favoriteGenre, null, null);
            filmsList = interestingFilms.Except(userFilms).ToList();

            if (filmsList != null || filmsList.Count() == 0)
            {
                var random = rd.Next(0, filmsList.Count());
                return filmsList[random];
            }

            var randomF = rd.Next(0, filmsList.Count());
            return GetFilms().ToList()[randomF];
        }

        public async Task<UserFilms> AddWatchLater(int userId, int filmId)
        {
            var user = GetUser().Where(u => u.Id == userId).FirstOrDefault();
            var film = GetFilms().Where(f => f.Id == filmId).FirstOrDefault();

            UserFilms newUF = new UserFilms
            {
                User = user,
                Film = film,
                FilmId = filmId,
                UserId = userId,
                Time = DateTime.Now,
                Status = nameof(FilmStatus.later)
            };
            var res = await _context.UserFilms.AddAsync(newUF);
            await _context.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<UserFilms> AddWatched(int userId, int filmId, int rating)
        {
            if (rating == 0) return null;
            var previousRatings = GetUserFilms().Where(f => f.UserId == userId && f.FilmId == filmId);
            if( previousRatings != null )
            {
                foreach(var rate in previousRatings)
                {
                    _context.UserFilms.Remove(rate);
                }
                _context.SaveChanges();
            }
            
            var user = GetUser().Where(u => u.Id == userId).FirstOrDefault();
            var film = GetFilms().Where(f => f.Id == filmId).FirstOrDefault();

            UserFilms newUF = new UserFilms
            {
                User = user,
                Film = film,
                FilmId = filmId,
                UserId = userId,
                Time = DateTime.Now,
                Status =  nameof(FilmStatus.seen),
                Rating = rating
            };
            var res = await _context.UserFilms.AddAsync(newUF);
            await _context.SaveChangesAsync();
            return res.Entity;
        }

        public User GetMostActiveUser(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetUserFilms().OrderByDescending(f => f.Time).Where(f => f.Time > starTime);
            var mostActive = recent.GroupBy(f => f.UserId).OrderByDescending(g => g.Count()).First().First().User;

            return mostActive;
        }

        public Actor GetBestActor(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime);
            var mostActive = recent.SelectMany(f => f.FilmActors)
                .GroupBy(g => g.Actor).OrderByDescending(f => f.Count()).Take(3);
            var mostActiveActors = mostActive.Select(f => f.Key).ToList();
            var mostActiveActorFilms = from ac in mostActiveActors
                join film in GetFilmActors() on ac.Id equals film.ActorId
                select new {ac.Id, film.Film.Rating};
            var bestActorId = mostActiveActorFilms.Where(a => a.Rating == mostActiveActorFilms.Max(f => f.Rating))
                .FirstOrDefault().Id;
            var bestActor = _context.Actor.Where(a => a.Id == bestActorId).FirstOrDefault();
            return bestActor;
        }

        public Actor GetTopRatedActor()
        {
            var topFilms = GetFilms().OrderByDescending(f => f.Rating).Take(100);
            var topActor = topFilms.SelectMany(f => f.FilmActors).GroupBy(d => d.ActorId)
                .OrderByDescending(a => a.Count()).First().Key;
            return _context.Actor.Where(ac => ac.Id == topActor).First();
        }

        public Director GetBestDirector(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime);
            var mostActive = recent
                .GroupBy(g => g.DirectorId).OrderByDescending(f => f.Count()).Take(3);
            var mostActiveDirector = mostActive.Select(f => f.Key).ToList();
            var mostActiveDirectorFilms = from ac in mostActiveDirector
                join dir in recent on ac equals dir.DirectorId
                select new {ac, dir.Rating};
            var bestDirectorId = mostActiveDirectorFilms
                .Where(a => a.Rating == mostActiveDirectorFilms.Max(f => f.Rating)).FirstOrDefault().ac;
            var bestDirector = _context.Director.Where(a => a.Id == bestDirectorId).FirstOrDefault();
            return bestDirector;
        }

        public Director GetTopRatedDiretcor()
        {
            var topFilms = GetFilms().OrderByDescending(f => f.Rating).Take(100);
            var topDirector = topFilms.GroupBy(d => d.Director).OrderByDescending(a => a.Count()).First().Key;
            return topDirector;
        }

        public Film GetTopRatedFilm()
        {
            var topFilm = GetFilms().OrderByDescending(f => f.Rating).First();
            return topFilm;
        }

        public IEnumerable<Film> GetBestFilm(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime);
            var views = from rec in recent
                join view in GetUserFilms() on rec.Id equals view.FilmId
                select view;
            var topIds = views.GroupBy(f => f.FilmId).OrderByDescending(g => g.Count()).Take(10).Select(g => g.Key);
            var topRatings = from id in topIds
                join film in recent on id equals film.Id
                select new {id, film.Rating};

            var bestFilmsIds = topRatings.Where(rat => rat.Rating == topRatings.Max(r => r.Rating)).Select(f => f.id);

            var bestFilns = from id in bestFilmsIds
                join film in recent on id equals film.Id
                select film;

            return bestFilns.ToArray();
        }

        public Film GetAwardedFilm(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime);
            var topFilm = recent.OrderByDescending(f => f.FilmReward.Count).First();
            return topFilm;
        }

        public string GetMostPopularGenre(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetUserFilms().OrderByDescending(f => f.Time).Where(f => f.Time > starTime);
            var topGenre = recent.GroupBy(r => r.Film.Genre).OrderByDescending(g => g.Count()).First().Key;
            return topGenre;
        }

        public FilmReward GetTopReward(DateTime? starTime)
        {
            if (starTime == null) starTime = DateTime.Now.AddYears(-1);
            var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime);
            var awards = recent.SelectMany(f => f.FilmReward).GroupBy(r => r.RewardName);
            var topRate = awards.Where(g =>
                g.Average(aw => aw.Film.Rating) == awards.Max(gg => gg.Average(asdf => asdf.Film.Rating))).First().Key;
            return GetFilmRewards().FirstOrDefault(r => r.RewardName == topRate);
        }

        public Film UpdateFilm(Film updated)
        {
            var res = _context.Film.Update(updated);
            _context.SaveChanges();
            return res.Entity;
        }
    }


    public interface IFilmService
    {
        CinematequeContext Context { get; }
        IEnumerable<Film> SearchFilms(string name, string genre, string director, string actor);
        Task<Film> AddFilm(Film film);
        Film GetSuggestion(string username);
        Task<UserFilms> AddWatchLater(int userId, int filmId);
        Task<UserFilms> AddWatched(int userId, int filmId, int rating);
        Actor GetBestActor(DateTime? starTime);
        User GetMostActiveUser(DateTime? starTime);
        Actor GetTopRatedActor();
        Director GetBestDirector(DateTime? starTime);
        Director GetTopRatedDiretcor();
        Film GetTopRatedFilm();
        IEnumerable<Film> GetBestFilm(DateTime? starTime);
        Film GetAwardedFilm(DateTime? starTime);
        string GetMostPopularGenre(DateTime? starTime);
        FilmReward GetTopReward(DateTime? starTime);
        Film UpdateFilm(Film updated);
        Task<FilmActors> AddFilmActor(FilmActors film);
        Task<FilmReward> AddAwards(FilmReward film);
        IQueryable<User> GetUser();
        IQueryable<FilmReward> GetFilmRewards();
        IQueryable<FilmActors> GetFilmActors();
        IQueryable<UserFilms> GetUserFilms();
        IQueryable<Film> GetFilms();
        IEnumerable<Film> GetHistory(int userId);
        IEnumerable<Film> GetWatchLater(int userId);
    }

    public enum FilmStatus
    {
        seen,
        later
    }
}
