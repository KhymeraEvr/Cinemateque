using System.Threading.Tasks;
using Cinemateque.DataAccess.Models.Movie;

namespace MovieData.Services
{
   public interface IRatingPredictionService
   {
      Task<MovieDataEntity> GetDataModel(int movieId);
      Task GetMovieRatingPrediction(int movieId);
   }
}