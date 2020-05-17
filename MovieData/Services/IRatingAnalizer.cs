using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemateque.DataAccess.Models;
using MoviesProcessing.Models;

namespace MovieData.Services
{
   public interface IRatingAnalizer
   {
      Task AnalizeActors(IEnumerable<CastModel> casts);
      Task<Actor> GetActorRating( Actor entity, CastModel actor);
      Task AnalizeCrew(CrewModel crew);
      Task<IEnumerable<string>> GetProductionCompanies();
   }
}