using Cinemateque.Data;
using Cinemateque.DataAccess.Models;
using Cinemateque.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private int _userId;

        private int UserID
        {
            get
            {
                var claims = User.Identity as ClaimsIdentity;
                _userId = _userService.DecodeTokenForClaims(Request.Cookies["Token"],HttpContext);
                return _userId;
            }
        }

        public HomeController(IFilmService service, IUserService userService)
        {
            _serv = service;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
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
            return View("FilmTable", fs);
        }

        [HttpPost("search")]
        public IActionResult SearchFilm([FromBody] SearchModel model)
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
            return Ok(_serv.Context.Actor.ToList().Select(a => new { Id = a.Id, Name = a.ActorName, Rating = a.Rating }));
        }

        [HttpGet("Home/directors")]
        public IActionResult GetDirectors()
        {
            return Ok(_serv.Context.Director.ToList().Select( a => new { a.Id, a.DirectorName, a.Rating}));
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
            return Ok(_serv.GetUser().ToList().Select( a => new { a.Id, a.UserName, a.Rating, a.Role }));
        }

        [HttpGet("Home/UserFilms")]
        public IActionResult GetUserFlms()
        {
            return Ok(_serv.GetUserFilms().ToList().Select( a => new { a.Id, a.Film.FilmName,a.User.UserName, a.Status, a.Rating, a.Time }));
        }

        [HttpGet("Home/awards")]
        public IActionResult GetAwards()
        {
            return Ok(_serv.GetFilmRewards().ToList().Select( a => new { a.Id, a.RewardName, a.Film.FilmName, a.Date }));
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
            foreach(var fl in suggestions)
            {
                var film = MapToViewModel(fl);
                fs.Add(film);
            }          

            return View("FilmTable", fs);
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

        [HttpGet("bestActor/{date}")]
        public IActionResult GetBestActor([FromRoute] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            var a = _serv.GetBestActor(datetime);
            return Ok(new { Name = a.ActorName, Rate = a.Rating });

        }

        [HttpGet("activeUser/{date}")]
        public IActionResult GetMostActiveUser([FromRoute] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            var a = _serv.GetMostActiveUser(datetime);
            return Ok(new { Name = a.UserName, Rate = a.Rating });
        }

        [HttpGet("ratedActor")]
        public IActionResult GetTopRatedActor()
        {
            var a = _serv.GetTopRatedActor();
            return Ok(new { Name = a.ActorName, Rate = a.Rating });
        }
        [HttpGet("favorites")]
        public IActionResult GetFavorites()
        {
            var a = _serv.GetFavorites(UserID);
            return Ok( a );
        }
        [HttpGet("bestDirector/{date}")]
        public IActionResult GetBestDirector([FromRoute] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            var a = _serv.GetBestDirector(datetime);
            return Ok(new { Name = a.DirectorName, Rate = a.Rating });
        }
        [HttpGet("ratedDirector")]
        public IActionResult GetTopRatedDiretcor()
        {
            var a = _serv.GetTopRatedDiretcor();
            return Ok(new { Name = a.DirectorName, Rate = a.Rating });
        }
        [HttpGet("ratedFilm")]
        public IActionResult GetTopRatedFilm()
        {
            var bestFilms = _serv.GetTopRatedFilm();
            List<FilmViewModel> fs = new List<FilmViewModel>();
            fs.Add(MapToViewModel(bestFilms));

            return View("FilmTable", fs);
        }

        public IActionResult GetBestFilm([FromForm] SearchModel date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date.Date);
            datetime = DateTime.Now.AddYears(-20);
            var bestFilms = _serv.GetBestFilm(datetime);
            List<FilmViewModel> fs = new List<FilmViewModel>();
            foreach (var f in bestFilms)
            {
                fs.Add(MapToViewModel(f));
            }

            return View("FilmTable", fs);
        }

        public IActionResult GetAwardedFilm([FromRoute] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            datetime = DateTime.Now.AddYears(-20);
            var bestFilms = _serv.GetAwardedFilm(datetime);
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
        [HttpGet("topReward/{date}")]
        public IActionResult GetTopReward([FromRoute] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            var a = _serv.GetTopReward(datetime);
            return Ok(new { Name = a.RewardName });
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
                Awards = _serv.GetFilmRewards().Where(r => r.FilmId == film.Id).Select(w => w.RewardName).ToArray()
            };
        }
    }
}