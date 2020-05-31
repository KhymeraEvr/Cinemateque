using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cinemateque.Data;
using Cinemateque.DataAccess.Models;
using Cinemateque.Models;
using Microsoft.AspNetCore.Mvc;
using MoviesProcessing.Models;
using MoviesProcessing.Services;

namespace Cinemateque.Controllers
{
   public class HomeController : Controller
   {
      private readonly IFilmService _serv;
      private readonly IUserService _userService;
      private readonly IMovieApiService _movieApiService;
      private readonly ICachedGenresService _genres;

      private int _userId;

      private int UserID
      {
         get
         {
            var claims = User.Identity as ClaimsIdentity;
            _userId = Convert.ToInt32(claims.FindFirst(ClaimTypes.Name)?.Value);
            return _userId;
         }
      }

      public HomeController(
         IFilmService serv,
         IUserService userService,
         IMovieApiService movieApiService,
         ICachedGenresService genres)
      {
         _serv = serv;
         _userService = userService;
         _movieApiService = movieApiService;
         _genres = genres;
      }

      public async Task<IActionResult> Index()
      {
         return View();
      }

      public async Task<IActionResult> Discover(int page)
      {
         var movies = await _movieApiService.GetDiscoverFilms();
         await SetUpGenres(movies);
         await _movieApiService.AddCreditsToMoves(movies);
         return View(movies);
      }

      public async Task<IActionResult> TopMovies()
      {
         var movies = await _movieApiService.GetTopRatedMoves(1);
         await SetUpGenres(movies);
         await _movieApiService.AddCreditsToMoves(movies);
         return View("Discover", movies.ToList());
      }

      public async Task<IActionResult> Upcoming()
      {
         var movies = await _movieApiService.GetUpdcoming(1);
         await SetUpGenres(movies);
         await _movieApiService.AddCreditsToMoves(movies);
         return View("Discover", movies.ToList());
      }

      public async Task<IActionResult> Search()
      {
         return View("Index");
      }

      public IActionResult Library()
      {
         return View();
      }

      [HttpPost("search")]
      public async Task<IActionResult> SearchFilm([FromBody] SearchModel model)
      {
         var movies = new List<Movie>();
         if (!string.IsNullOrEmpty( model.Name ))
         {
            var moviesName = await _movieApiService.Search(model.Name);
            movies.AddRange(moviesName);
         }

         if (!string.IsNullOrEmpty(model.Director)
            || !string.IsNullOrEmpty(model.Actor)
            || !string.IsNullOrEmpty(model.Genre)
            || model.Date.HasValue)
         {
            var fhdskjhkgddfsh = await _movieApiService.SearchByModel(model);
            movies.AddRange(fhdskjhkgddfsh);
         }

         await SetUpGenres(movies);
         await _movieApiService.AddCreditsToMoves(movies);
         return View("Discover", movies.ToList());
      }

      [Route("History")]
      public IActionResult History()
      {
         var history = _serv.GetHistory(UserID);

         return View(history.ToList());
      }

      [Route("WatchLater")]
      public IActionResult WatchLater()
      {
         var later = _serv.GetWatchLater(UserID);
         return View(later.ToList());
      }

      [Route("AddFilmForm")]
      public IActionResult AddFilmForm()
      {
         return View("AddFilm");
      }

      //[HttpGet("Home/directors")]
      //public IActionResult GetDirectors()
      //{
      //   return Ok(_serv.Context.Director.ToList().Select(a => new { a.Id, a.DirectorName, a.Rating }));
      //}

      [HttpGet("Home/user")]
      public IActionResult GetUsers()
      {
         return Ok(_serv.Context.User.ToList().Select(a => new { a.Id, a.UserName, a.Rating, a.Role }));
      }

      [HttpGet("Home/UserFilms")]
      public IActionResult GetUserFlms()
      {
         return Ok(_serv.Context.UserFilms.ToList().Select(a => new { a.Id, a.Film.FilmName, a.User.UserName, a.Status, a.Rating, a.Time }));
      }

      [HttpGet("Home/later/{filmId}")]
      public IActionResult AddWatchLater([FromRoute] int filmId)
      {
         return Ok(_serv.AddWatchLater(Convert.ToInt32(UserID), filmId)?.Id);
      }

      [HttpGet("rate/{filmId}/{rating}")]
      public IActionResult AddWatched([FromRoute] int filmId, [FromRoute] int rating)
      {
         return Ok(_serv.AddWatched(Convert.ToInt32(UserID), filmId, rating).Id);
      }

      private async Task SetUpGenres(IEnumerable<Movie> movies)
      {
         foreach (var movie in movies)
         {
            movie.Genres = new List<string>();
            foreach (var id in movie.GenreIds)
            {
               movie.Genres.Add(await _genres.GetGenreById(id));
            }
         }
      }
   }
}