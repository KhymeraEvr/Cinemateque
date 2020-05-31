using System;
using System.Linq;
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
      private readonly IYouTubeService _youTubeService;
      

      public AnalyzeController(
         IMovieApiService movieApiService,
        IRatingAnalizer analizer,
        IMovieDataService dataService,
        IRatingPredictionService ratingPrediction,
        IYouTubeService youTubeService)
      {
         _movieApiService = movieApiService;
         _analizer = analizer;
         _dataService = dataService;
         _ratingPrediction = ratingPrediction;
         _youTubeService = youTubeService;
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

      [HttpGet("movies/data/discover/{page}")]
      public async Task<IActionResult> GetDiscoiverMoviesPageData(int page)
      {
         for(int i = 20; i < 200; i++)
         {
            await _ratingPrediction.GetDiscoverPageData(i);

         }

         return Ok();
      }

      [HttpGet("movies/data/top/{page}")]
      public async Task<IActionResult> GetTopMoviesPageData(int page)
      {
         await _ratingPrediction.GetTopPageData(page);

         return Ok();
      }

      [HttpGet("movies/predict/{movieId}")]
      public async Task<IActionResult> GetMovieRatingPrediction(int movieId)
      {
         await _ratingPrediction.GetMovieRatingPrediction( movieId );

         return Ok();
      }

      [HttpGet("movies/trailer/{name}")]
      public async Task<IActionResult> GetMovieTrailerData(string name)
      {
         var searchName = string.Concat(name.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

         var moviesawait = await _youTubeService.SearchStats(searchName);

         return Ok(moviesawait);
      }
   }
}