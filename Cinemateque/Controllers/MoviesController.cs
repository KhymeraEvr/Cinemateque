//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Cinemateque.Data;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Cinemateque.Controllers
//{
//   [Route("api/[controller]")]
//   [ApiController]
//   public class MoviesController : ControllerBase
//   {
//      private readonly IMovieApiService _movieService;

//      public MoviesController(IMovieApiService movieService )
//      {
//         _movieService = movieService;
//      }

//      [HttpGet("discover")]
//      public async Task<IActionResult> Discover()
//      {
//         await _movieService.GetDiscoverFilms();

//         return Ok();
//      }
//   }
//}