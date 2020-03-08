using Cinemateque.DataAccess;
using Cinemateque.DataAccess.Models;
using MoviesProcessing.Models;
using MoviesProcessing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var model = await GetActorRating(actor);
            var actorEntity = _context.Actors.FirstOrDefault(a => a.ActorName == actor.Name);
            if (actorEntity != null)
            {
               var totalChecks = model.FilmsChecked + actorEntity.FilmsChecked;
               model.Rating = (actorEntity.Rating + model.Rating) / totalChecks;
            }
            var addResult = await _context.Actors.AddAsync(model);
         }

         await _context.SaveChangesAsync();
      }

      public async Task<Actor> GetActorRating(CastModel actor)
      {
         var actorMovies = await _movieService.GetActorMovies(actor.Id.ToString());
         double totalRatings = 0;
         foreach (var film in actorMovies)
         {
            totalRatings += film.VoteAverage;
         }

         var moviesCount = actorMovies.Count();
         var average = totalRatings / moviesCount;
         var actorModel = new Actor
         {
            ActorName = actor.Name,
            FilmsChecked = moviesCount,
            Rating = average
         };
         
         return actorModel;
      }
   }
}
