using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieData.Services;
using MoviesProcessing.Services;

namespace Cinemateque.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AnalyzeController : ControllerBase
   {
      private readonly IMovieApiService _movieApiService;
      private readonly IRatingAnalizer _analizer;
      private readonly IMovieDataService _dataService;
      private readonly IRatingPredictionService _ratingPrediction;

      public AnalyzeController(
         IMovieApiService movieApiService,
        IRatingAnalizer analizer,
        IMovieDataService dataService,
        IRatingPredictionService ratingPrediction)
      {
         _movieApiService = movieApiService;
         _analizer = analizer;
         _dataService = dataService;
         _ratingPrediction = ratingPrediction;
      }

      [HttpGet("actors/{page}")]
      public async Task AnalizeActorPage(int page)
      {
         var topMovies = await _movieApiService.GetTopRatedMoves(page);
         foreach (var movie in topMovies)
         {
            var credits = await _movieApiService.GetCredits(movie.Id.ToString());
            var cast = credits.Cast;

            await _analizer.AnalizeActors(cast);
         }
      }

      [HttpGet("actors/data/{actorName}")]
      public async Task GetActorData(string actorName)
      {
         await _dataService.GetActorCsv(actorName);
      }

      [HttpGet("movies/predict/{movieId}")]
      public async Task<IActionResult> GetMovieRatingPrediction(int movieId)
      {
         await _ratingPrediction.GetMovieRatingPrediction( movieId );

         return Ok();
      }

   }
}