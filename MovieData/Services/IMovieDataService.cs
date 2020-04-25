using System.Threading.Tasks;

namespace MovieData.Services
{
   public interface IMovieDataService
   {
      Task GetActorCsv(string actorName);
   }
}