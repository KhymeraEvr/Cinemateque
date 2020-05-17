using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.DataAccess;
using Cinemateque.DataAccess.Models;
using Cinemateque.DataAccess.Models.Crew;
using CsvHelper;
using MoviesProcessing.Models;
using MoviesProcessing.Services;

namespace MovieData.Services
{
   public class RatingAnalizer : IRatingAnalizer
   {
      private readonly IMovieApiService _movieService;
      private readonly CinematequeContext _context;

      public RatingAnalizer(IMovieApiService movieApiService, CinematequeContext context)
      {
         _movieService = movieApiService;
         _context = context;
      }

      public async Task AnalizeActors(IEnumerable<CastModel> casts)
      {
         foreach (var actor in casts)
         {
            var model = await AnalizeActor(actor);

            if (model == null) continue;  

            if (model.Ratings.Count >= 10)
            {
               var updateResult = await _context.Actors.AddAsync(model);
            }
         }

         await _context.SaveChangesAsync();
      }

      public async Task<Actor> AnalizeActor(CastModel actor)
      {
         var actorEntity = _context.Actors.FirstOrDefault(a => a.ActorName == actor.Name);
         if (actorEntity == null)
         {
            actorEntity = new Actor
            {
               ActorName = actor.Name
            };
         }
         else
         {
            return null;
         }

         var model = await GetActorRating(actorEntity, actor);

         return model;
      }

      public async Task AnalizeCrew(CrewModel crew)
      {
         var crewEntity = _context.CrewMembers.FirstOrDefault(a => a.Name == crew.Name);
         if (crewEntity == null)
         {
            crewEntity = new CrewMember
            {
               Name = crew.Name
            };
         }
         else
         {
            return;
         }

         var model = await GetCrewRatings(crewEntity, crew);

         if (model.Ratings.Count >= 5)
         {
            var updateResult = await _context.CrewMembers.AddAsync(model);
         }

         await _context.SaveChangesAsync();
      }

      public async Task<Actor> GetActorRating(Actor entity, CastModel actor)
      {
         var ratingModels = new List<ActorRatingEntry>();

         var actorMovies = await _movieService.GetActorMovies(actor.Id.ToString());
         foreach (var film in actorMovies)
         {
            if (string.IsNullOrEmpty(film.ReleaseDate))
            {
               continue;
            }
            var entry = new ActorRatingEntry
            {
               Date = DateTime.Parse(film.ReleaseDate),
               Rating = film.VoteAverage
            };

            ratingModels.Add(entry);
         }

         entity.Ratings = ratingModels;

         return entity;
      }

      public async Task<CrewMember> GetCrewRatings(CrewMember entity, CrewModel crew)
      {
         var ratingModels = new List<CrewRatingEntry>();

         var crewMovies = await _movieService.GetCrewMovies(crew.Id.ToString());

         foreach (var film in crewMovies)
         {
            if (string.IsNullOrEmpty(film.ReleaseDate))
            {
               continue;
            }
            var entry = new CrewRatingEntry
            {
               Date = DateTime.Parse(film.ReleaseDate),
               Rating = film.VoteAverage
            };

            ratingModels.Add(entry);
         }

         entity.Ratings = ratingModels;

         return entity;
      }

      public async Task<IEnumerable<string>> GetProductionCompanies()
      {
         var movies1 = await _movieService.GetTopRatedMoves(1);
         var movies2 = await _movieService.GetTopRatedMoves(2);
         var movies3 = await _movieService.GetTopRatedMoves(3);
         var movies4 = await _movieService.GetTopRatedMoves(4);
         var movies5 = await _movieService.GetTopRatedMoves(5);

         var movies = movies1.Concat(movies2).Concat(movies3);

         var moviesDetails = new List<MovieDetails>();

         foreach (var movie in movies)
         {
            var details = await _movieService.GetMovieDetails(movie.Id.ToString());
            moviesDetails.Add(details);
         }

         var companies = moviesDetails.SelectMany(x => x.Companies).OrderBy(x => x.Name).ToList();

         var compsCount = new Dictionary<string, int>();

         for (int i = 0; i < companies.Count; i++)
         {
            int j = i;
            compsCount[companies[j].Name] = 1;
            while (j < companies.Count && companies[j].Name == companies[i].Name)
            {
               compsCount[companies[j].Name] += 1;
               j++;
            }
            i = j - 1;
         }

         var comps = compsCount.Where(x => x.Value > 2).Select(x => x.Key);

         var res = new
         {
            Companies = comps
         };

         var fileDir = $"..\\companiesList.csv";

         using (var writer = new StreamWriter(fileDir))
         using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
         {
            csv.WriteRecords(res.Companies);
         }

         return compsCount.Keys;
      }
   }
}
