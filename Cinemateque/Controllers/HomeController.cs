using Cinemateque.Data;
using Cinemateque.DataAccess.Models;
using Cinemateque.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieData.Services;
using MoviesProcessing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinemateque.Controllers
{
   public class HomeController : Controller
   {
      private readonly IFilmService _serv;
      private readonly IUserService _userService;
      private readonly IOrderService _orderServ;
      private readonly IRatingAnalizer _analizer;
      private readonly IMovieApiService _movieApiService;
      private int _userId;



      private int UserID
      {
         get
         {
            var claims = User.Identity as ClaimsIdentity;
            _userId = _userService.DecodeTokenForClaims(Request.Cookies["Token"], HttpContext);
            return _userId;
         }
      }

      public HomeController(
         IFilmService service,
         IUserService userService,
         IOrderService orderService,

         IRatingAnalizer analizer,
         IMovieApiService movieServ)
      {
         _userService = userService;
         _orderServ = orderService;

         _movieApiService = movieServ;
         _analizer = analizer;
      }

      //[Authorize]
      public async Task<IActionResult> Index()
      {
         var topMovies = await _movieApiService.GetTopRatedMoves(1);
         foreach (var movie in topMovies)
         {
            var credits = await _movieApiService.GetCredits(movie.Id.ToString());
            var cast = credits.Cast;

            await _analizer.AnalizeActors(cast);
         }

         //var films = _serv.GetFilms();
         //List<FilmViewModel> fs = new List<FilmViewModel>();
         //foreach (var f in films)
         //{
         //    fs.Add(MapToViewModel(f));
         //}
         return View("FilmPanels");
      }

      [Authorize]
      public async Task<IActionResult> Cart()
      {
         var model = await _orderServ.GetCart(UserID);

         return View(model);
      }

      [HttpGet]
      public IActionResult Edit([FromQuery]string name)
      {
         if (name == null)
         {
            name = _serv.GetUser(UserID).UserName;
         }
         var user = _serv.GetUser(name);
         var orders = _orderServ.GetUserOrders(user.Id);
         var model = new EditViewModel
         {
            Oreders = orders.ToList(),
            User = user.UserName
         };
         return View("EditOrder", model);
      }

      [Authorize(Roles = "Admin")]
      [HttpPost]
      public async Task EditOrder([FromBody] UpdateOrderModel model)
      {
         var order = await _orderServ.GetOrder(Convert.ToInt32(model.OrderId));
         order.Status = model.newStatus;
         await _orderServ.UpdateOrder(order);
      }

      [HttpGet]
      public async Task AddToCart([FromQuery]int prodId)
      {
         var order = new Order
         {
            FilmId = prodId,
            Status = "Active",
            UserId = UserID
         };
         _orderServ.AddOrder(order).Wait();
      }


      public IActionResult Library()
      {
         return View();
      }

      [Route("SearchTable")]
      public IActionResult SearchTable(SearchModel model)
      {
         var fims = _serv.SearchFilms(model.Name, model.Genre, model.Director, model.Actor).ToList();
         List<FilmViewModel> fs = new List<FilmViewModel>();
         foreach (var f in fims)
         {
            fs.Add(MapToViewModel(f));
         }
         return View("Search", fs);
      }

      public IActionResult SearchView()
      {
         var model = new List<FilmViewModel>();
         return View("Search", model);
      }

      public IActionResult Search([FromBody] SearchModel model)
      {
         return RedirectToAction("SearchTable", model);
      }

      [Route("History")]
      public IActionResult History()
      {
         var history = _serv.GetHistory(UserID);
         List<FilmViewModel> fs = new List<FilmViewModel>();
         foreach (var f in history)
         {
            fs.Add(MapToViewModel(f));
         }
         return View(fs);
      }

      [Route("WatchLater")]
      public IActionResult WatchLater()
      {
         var later = _serv.GetWatchLater(UserID);
         List<FilmViewModel> fs = new List<FilmViewModel>();
         foreach (var f in later)
         {
            fs.Add(MapToViewModel(f));
         }
         return View(fs);
      }

      [Route("AddFilmForm")]
      public IActionResult AddFilmForm()
      {
         return View("AddFilm");
      }

      [Route("UpdateFilmForm")]
      public IActionResult UpdateFilmForm()
      {
         return View("UpdateFilm");
      }

      [HttpGet("Home/actors")]
      public IActionResult GetActors()
      {
         return Ok(_serv.Context.Actors.ToList().Select(a => new { Id = a.Id, Name = a.ActorName, Rating = a.Rating }));
      }

      [HttpGet("Home/directors")]
      public IActionResult GetDirectors()
      {
         return Ok(_serv.Context.Directors.ToList().Select(a => new { a.Id, a.DirectorName, a.Rating }));
      }

      [HttpGet("Home/films")]
      public IActionResult GetFilms()
      {
         var films = _serv.GetFilms().ToList();
         List<FilmViewModel> fs = new List<FilmViewModel>();
         foreach (var f in films)
         {
            fs.Add(MapToViewModel(f));
         }
         return Ok(fs);
      }

      [HttpGet("Home/user")]
      public IActionResult GetUsers()
      {
         return Ok(_serv.GetUser().ToList().Select(a => new { a.Id, a.UserName, a.Rating, a.Role }));
      }

      [HttpGet("Home/UserFilms")]
      public IActionResult GetUserFlms()
      {
         return Ok(_serv.GetUserFilms().ToList().Select(a => new { a.Id, a.Film.FilmName, a.User.UserName, a.Status, a.Rating, a.Time }));
      }


      [HttpPost]
      public async Task<IActionResult> AddFilm([FromForm] AddFilmModel model)
      {

         await _serv.AddFilm(model);

         return RedirectToAction("AddFilmForm");
      }

      [HttpPost]
      public IActionResult UpdateFilm([FromForm] AddFilmModel model)
      {
         return Ok(_serv.UpdateFilm(model));
      }


      public IActionResult GetSuggestion()
      {
         List<FilmViewModel> fs = new List<FilmViewModel>();
         var suggestions = _serv.GetSuggestion(UserID);
         foreach (var fl in suggestions)
         {
            var film = MapToViewModel(fl);
            fs.Add(film);
         }

         return View("Search", fs);
      }

      [HttpGet("later/{filmId}")]
      public IActionResult AddWatchLater([FromRoute] int filmId)
      {
         return Ok(_serv.AddWatchLater(Convert.ToInt32(UserID), filmId)?.Id);
      }

      [HttpGet("rate/{filmId}/{rating}")]
      public IActionResult AddWatched([FromRoute] int filmId, [FromRoute] int rating)
      {
         return Ok(_serv.AddWatched(Convert.ToInt32(UserID), filmId, rating).Id);
      }

      [HttpDelete("delete/{filmId}")]
      public IActionResult DeleteFilm([FromRoute] int filmid)
      {
         _serv.DeleteFilm(filmid);
         return Ok();
      }


      [HttpGet("activeUser/{date}")]
      public IActionResult GetMostActiveUser([FromRoute] string date)
      {
         DateTime? datetime = null;
         if (date != null) datetime = Convert.ToDateTime(date);
         var a = _serv.GetMostActiveUser(datetime);
         return Ok(new { Name = a.UserName, Rate = a.Rating });
      }

      [HttpGet("favorites")]
      public IActionResult GetFavorites()
      {
         var a = _serv.GetFavorites(UserID);
         return Ok(a);
      }

      [HttpGet("ratedFilm")]
      public IActionResult GetTopRatedFilm()
      {
         var bestFilms = _serv.GetTopRatedFilm();
         List<FilmViewModel> fs = new List<FilmViewModel>();
         fs.Add(MapToViewModel(bestFilms));

         return View("FilmTable", fs);
      }


      [HttpGet("popGenre/{date}")]
      public IActionResult GetMostPopularGenre([FromRoute] string date)
      {
         DateTime? datetime = null;
         if (date != null) datetime = Convert.ToDateTime(date);
         var a = _serv.GetMostPopularGenre(datetime);
         return Ok(new { Name = a });
      }

      private FilmViewModel MapToViewModel(Film film)
      {

         var userFilm = _serv.GetUserFilms().Where(uf => uf.UserId == UserID && uf.FilmId == film.Id).FirstOrDefault();
         return new FilmViewModel
         {
            FilmId = film.Id,
            FilmName = film.FilmName,
            Genre = film.Genre,
            Director = film.Director.DirectorName,
            PremiereDate = film.PremiereDate.Value.ToShortDateString(),
            UserRating = userFilm != null ? (int)(userFilm.Rating ?? 0) : 0,
            Rating = film.Rating ?? 0,
            Actors = _serv.GetFilmActors().Where(f => f.FilmId == film.Id).Select(a => a.Actor.ActorName).ToArray(),
            Price = film.Price ?? 0.0,
            Discount = film.Discount ?? 0.0,
            Image = film.Image
         };
      }
   }
}