using System.Threading.Tasks;
using Cinemateque.DataAccess.Models.Movie;
using MoviesProcessing.Models;

namespace MovieData.Services
{
   public interface IMovieDataService
   {
      Task<string> GetActorCsv(string actorName, CastModel actorModel = null);

      Task<string> GetCrewCsv(string crewName);

      Task SaveMovie(MovieDataEntity movie);

      MovieDataEntity GetMovie(int id);
   }
}