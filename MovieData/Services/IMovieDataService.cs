using System.Threading.Tasks;
using Cinemateque.DataAccess.Models.Movie;

namespace MovieData.Services
{
   public interface IMovieDataService
   {
      Task<string> GetActorCsv(string actorName);

      Task<string> GetCrewCsv(string crewName);

      Task SaveMovie(MovieDataModel movie);

      MovieDataModel GetMovie(int id);
   }
}