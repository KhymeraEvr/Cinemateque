using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesProcessing.Services;

namespace Cinemateque.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class MoviesController : ControllerBase
   {
      private readonly IMovieApiService _movieService;

      public MoviesController(IMovieApiService movieService)
      {
         _movieService = movieService;
      }

      [HttpGet("discover")]
      public async Task<IActionResult> Discover()
      {
         var movies = await _movieService.GetDiscoverFilms();

         return Ok(movies);
      }

      [HttpGet("topMovies")]
      public async Task<IActionResult> GetTopMovies()
      {
         await _movieService.GetDiscoverFilms();

         return Ok();
      }
   }
}