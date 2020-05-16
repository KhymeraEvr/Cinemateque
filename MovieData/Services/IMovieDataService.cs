using System.Threading.Tasks;
using Cinemateque.DataAccess.Models.Movie;

namespace MovieData.Services
{
   public interface IMovieDataService
   {
      Task<string> GetActorCsv(string actorName);

      Task<string> GetCrewCsv(string crewName);

      Task SaveMovie(MovieDataEntity movie);

      MovieDataEntity GetMovie(int id);
   }
}