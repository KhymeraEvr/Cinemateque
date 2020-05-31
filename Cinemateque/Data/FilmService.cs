using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinemateque.DataAccess;
using Cinemateque.DataAccess.Models;
using Cinemateque.DataAccess.Models.Movie;
using Cinemateque.Models;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MovieData.Services;
using MoviesProcessing.Services;

namespace Cinemateque.Data
{
   public class FilmService : IFilmService // obsolete! move on with your life, use movieProcessing lib
   {
      private readonly CinematequeContext _context;
      private readonly IMovieApiService _movServ;
      private readonly IRatingPredictionService _predServ;

      public CinematequeContext Context => _context;

      public FilmService(
         CinematequeContext context,
         IMovieApiService movServ,
         IRatingPredictionService predServ)
      {
         _predServ = predServ;
         _movServ = movServ;
         _context = context;
      }

      public IQueryable<UserFilms> GetUserFilms()
      {
         return _context.UserFilms.Include(x => x.Film );
      }


      public IQueryable<User> GetUser()
      {
         return _context.User.Include(u => u.UserFilms);
      }

      public IEnumerable<Film> GetHistory(int userId)
      {
         var sho = GetUserFilms().ToArray();
         var history = GetUserFilms().Where(f => f.UserId == userId && f.Status == nameof(FilmStatus.seen)).Select(f => f.Film).ToArray();
         return history;
      }

      public IEnumerable<Film> GetWatchLater(int userId)
      {
         var history = GetUserFilms().Where(f => f.UserId == userId && f.Status == nameof(FilmStatus.later)).Select(f => f.Film).ToArray();
         return history;
      }

      public async Task<UserFilms> AddWatchLater(int userId, int filmId)
      {
         var user = GetUser().Where(u => u.Id == userId).FirstOrDefault();
         var film = await GetFilm(filmId);

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

      public async Task<Film> GetFilm(int id)
      {
         var film = _context.Film.FirstOrDefault(x => x.Id == id);
         if (film == null)
         {
            var movie = await _predServ.GetDataModel(id);
            film = MapMoviesToFilm(movie);
         }

         return film;
      }

      public async Task<UserFilms> AddWatched(int userId, int filmId, float rating)
      {
         if (rating == 0) return null;
         var previousRatings = GetUserFilms().Where(f => f.UserId == userId && f.FilmId == filmId);
         if (previousRatings != null)
         {
            foreach (var rate in previousRatings)
            {
               _context.UserFilms.Remove(rate);
            }

             // _context.SaveChanges();
         }

         var user = GetUser().Where(u => u.Id == userId).FirstOrDefault();
         var film = await GetFilm(filmId);

         var filmReviews = GetUserFilms().Where(u => u.FilmId == film.Id).Count();

         float updatedRate;

         if (film.Rating == null)
         {
            updatedRate = rating;
         }
         else
         {
            updatedRate = (film.Rating.Value * (float)filmReviews + rating) / ((float)filmReviews + (float)1);
         }

         //var difference = updatedRate - film.Rating.Value;
         //var koef = GetUserFilms().Where(u => u.UserId == userId).Count() / GetFilms().Count();
         //difference *= koef;
         //rating = film.Rating.Value + koef;

         if (previousRatings.Count() != 0 && filmReviews == 1)
         {
            updatedRate = rating;
         }


         film.Rating = updatedRate;


         //var resUpdate = _context.Film.Update(film);
         //await _context.SaveChangesAsync();
         UserFilms newUF = new UserFilms
         {
            FilmId = filmId,
            UserId = userId,
            Time = DateTime.Now,
            Status = nameof(FilmStatus.seen),
            Rating = rating,
            Film = film
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
         return null;

         //if (starTime == null) starTime = DateTime.Now.AddYears(-1);
         //var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime).Take(100);
         //var mostActive = recent.SelectMany(f => f.FilmActors)
         //    .GroupBy(g => g.ActorId).OrderByDescending(f => f.Count()).Take(3);
         //var mostActiveActors = mostActive.Select(f => f.Key).ToList();
         //var mostActiveActorFilms = from ac in mostActiveActors
         //                           join film in GetFilmActors() on ac equals film.ActorId
         //                           select new { ac, film.Film.Rating };
         //var bestActorId = mostActiveActorFilms.Where(a => a.Rating == mostActiveActorFilms.Max(f => f.Rating))
         //    .FirstOrDefault();
         //var bestActor = _context.Actor.Where(a => a.Id == bestActorId.ac).FirstOrDefault();
         //return bestActor;
      }

      public Actor GetTopRatedActor()
      {
         return null;

         //var topFilms = GetFilms().OrderByDescending(f => f.Rating).Take(100);
         //var topActor = topFilms.SelectMany(f => f.FilmActors).GroupBy(d => d.ActorId)
         //    .OrderByDescending(a => a.Count()).First().Key;
         //return _context.Actor.Where(ac => ac.Id == topActor).First();
      }

      public Director GetBestDirector(DateTime? starTime)
      {
         return null;

         //if (starTime == null) starTime = DateTime.Now.AddYears(-1);
         //var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime);
         //var mostActive = recent
         //    .GroupBy(g => g.DirectorId).OrderByDescending(f => f.Count()).Take(3);
         //var mostActiveDirector = mostActive.Select(f => f.Key).ToList();
         //var mostActiveDirectorFilms = from ac in mostActiveDirector
         //                              join dir in recent on ac equals dir.DirectorId
         //                              select new { ac, dir.Rating };
         //var bestDirectorId = mostActiveDirectorFilms
         //    .Where(a => a.Rating == mostActiveDirectorFilms.Max(f => f.Rating)).FirstOrDefault().ac;
         //var bestDirector = _context.Director.Where(a => a.Id == bestDirectorId).FirstOrDefault();
         //return bestDirector;
      }


      public IEnumerable<Film> GetBestFilm(DateTime? starTime)
      {
         //if (starTime == null) starTime = DateTime.Now.AddYears(-1);
         //var recent = GetFilms().OrderByDescending(f => f.PremiereDate).Where(f => f.PremiereDate > starTime).Take(100);
         //var views = from rec in recent
         //            join view in GetUserFilms() on rec.Id equals view.FilmId
         //            select view;
         //var topIds = views.GroupBy(f => f.FilmId).OrderByDescending(g => g.Count()).Take(10).Select(g => g.Key).ToList();
         //var topRatings = from id in topIds
         //                 join film in recent on id equals film.Id
         //                 select new { id, film.Rating };

         //var bestFilmsIds = topRatings.Where(rat => rat.Rating == topRatings.Max(r => r.Rating)).Select(f => f.id);

         //var bestFilns = from id in bestFilmsIds
         //                join film in recent on id equals film.Id
         //                select film;

         //return bestFilns.ToArray();
         return null;
      }


      public string GetMostPopularGenre(DateTime? starTime)
      {
         //if (starTime == null) starTime = DateTime.Now.AddYears(-1);
         //var recent = GetUserFilms().OrderByDescending(f => f.Time).Where(f => f.Time > starTime).Take(100);
         //var topGenre = recent.GroupBy(r => r.Film.Genre).OrderByDescending(g => g.Count()).First().Key;
         //return topGenre;
         return "";
      }

      public async Task<Film> UpdateFilm(AddFilmModel model)
      {
         return null;

         //var toUpdate = GetFilms().Where(f => f.FilmName == model.FilmName).FirstOrDefault();
         //if (toUpdate == null) return null;
         //var actors = model.Actors?.Split(", ").ToList() ?? new List<string>();

         //if (!_context.Director.Any(d => d.DirectorName == model.Director))
         //{
         //   _context.Director.Add(new Director
         //   {
         //      DirectorName = model.Director
         //   });
         //   await _context.SaveChangesAsync();
         //}

         //var directorID = _context.Director.First(d => d.DirectorName == model.Director).Id;

         //toUpdate.DirectorId = directorID;
         //toUpdate.FilmName = model.FilmName;
         //toUpdate.Genre = model.Genre;
         //toUpdate.PremiereDate = model.PremiereDate;

         //var res = _context.Film.Update(toUpdate).Entity;
         //_context.SaveChanges();

         //foreach (var ac in actors)
         //{
         //   int actorId;
         //   if (!_context.Actor.Any(a => a.ActorName == ac))
         //   {
         //      var newAct = _context.Actor.Add(new Actor
         //      {
         //         ActorName = ac
         //      });
         //      await _context.SaveChangesAsync();
         //   }
         //   actorId = _context.Actor.Where(a => a.ActorName == ac).First().Id;

         //   var toAdd = new FilmActors
         //   {
         //      ActorId = actorId,
         //      FilmId = res.Id
         //   };
         //   AddFilmActor(toAdd).Wait();
         //}

         //return res;
      }

      public async Task DeleteFilm(int filmId)
      {
         return;

         //var film = _context.Film.Find(filmId);
         //var filmActors = _context.FilmActors.Where(f => f.FilmId == film.Id)?.ToList() ?? new List<FilmActors>();
         //foreach (var f in filmActors)
         //{
         //   _context.FilmActors.Remove(f);
         //}
         //await _context.SaveChangesAsync();
         //if (film != null)
         //{
         //   _context.Film.Remove(film);
         //   await _context.SaveChangesAsync();
         //}
      }



      public Favorites GetFavorites(int userID)
      {
         //var user = GetUser().Where(u => u.Id == userID);
         //var userFilms = GetUserFilms().Where(f => f.User.Id == userID).Select(f => f.Film)
         //    .Include(f => f.Director)
         //    .Include(f => f.FilmActors)
         //    .ThenInclude(f => f.Actor).ToList();

         //var favoriteGenre = userFilms.GroupBy(f => f.Genre)
         //    .OrderByDescending(g => g.Count()).First().Select(f => f.Genre).First();

         //var favoriteDirector = userFilms.GroupBy(f => f.Director.DirectorName)
         //    .OrderByDescending(g => g.Count()).First().Select(f => f.Director.DirectorName).First();

         //var favoriteActor = userFilms.SelectMany(f => f.FilmActors)
         //    .GroupBy(a => a.Actor.Id).OrderByDescending(a => a.Count()).First()
         //    .Select(f => f.Actor.ActorName).First();

         //var res = new Favorites
         //{
         //   Actor = favoriteActor,
         //   Director = favoriteDirector,
         //   Genre = favoriteGenre
         //};
         //return res;
         return new Favorites();
      }

      public User GetUser(int name)
      {
         var users = GetUser().FirstOrDefault(u => u.Id == name);
         return users;
      }

      public User GetUser(string name)
      {
         var users = GetUser().FirstOrDefault(u => u.UserName == name);
         return users;
      }

      public Film MapMoviesToFilm(MovieDataEntity ent)
      {
         var actors = ent.ActorsCsvs.Split(";").Select(x => AddSpacesToSentence(x, true));
         var actorsSt = string.Join(", ", actors);

         var crew = ent.CrewCsvs.Split(";").Select(x => AddSpacesToSentence(x, true));
         var crewSt = string.Join(", ", crew);
         var genres = ent.Genres.Replace(";", ", ");

         var film = new Film
         {
            FilmName = ent.Title,
            Actors = actorsSt,
            Director = crewSt,
            Genres = ent.Genres,
            PremiereDate = ent.ReleaseDate,
            Movieid = ent.MovieId
         };

         return film;
      }

      string AddSpacesToSentence(string text, bool preserveAcronyms)
      {
         if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
         StringBuilder newText = new StringBuilder(text.Length * 2);
         newText.Append(text[0]);
         for (int i = 1; i < text.Length; i++)
         {
            if (char.IsUpper(text[i]))
               if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                   (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                    i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                  newText.Append(' ');
            newText.Append(text[i]);
         }
         return newText.ToString();
      }
   }


   public interface IFilmService
   {
      CinematequeContext Context { get; }
      Task<UserFilms> AddWatchLater(int userId, int filmId);
      Task<UserFilms> AddWatched(int userId, int filmId, float rating);
      IQueryable<User> GetUser();
      IQueryable<UserFilms> GetUserFilms();
      IEnumerable<Film> GetHistory(int userId);
      IEnumerable<Film> GetWatchLater(int userId);
      User GetUser(int name);
      User GetUser(string name);
   }

   public enum FilmStatus
   {
      seen,
      later
   }
}

