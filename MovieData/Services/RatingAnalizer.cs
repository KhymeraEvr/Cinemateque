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
               continue;
            }

            var model = await GetActorRating(actorEntity, actor);

            if (model.Ratings.Count >= 10)
            {
               var updateResult = await _context.Actors.AddAsync(model);
            }
         }

         await _context.SaveChangesAsync();
      }

      public async Task<Actor> GetActorRating( Actor entity, CastModel actor)
      {
         var ratingModels = new List<ActorRatingEntry>();

         var actorMovies = await _movieService.GetActorMovies(actor.Id.ToString());         
         foreach (var film in actorMovies)
         {
            if( string.IsNullOrEmpty(film.ReleaseDate) )
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
   }
}
