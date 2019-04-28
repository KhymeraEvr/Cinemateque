using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.Data;
using Microsoft.AspNetCore.Mvc;
using Cinemateque.Models;
using System.Security.Claims;

namespace Cinemateque.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilmService _serv;

        public HomeController(IFilmService service)
        {
            _serv = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("films")]
        public IActionResult GetAllFilms()
        {
            var fims = _serv.GetFilms().ToList();
            List<FilmViewModel> fs = new List<FilmViewModel>();

            foreach( var f in fims)
            {
                fs.Add(MapToViewModel(f));
            }
            return Ok(fs.ToArray());
        }

        [HttpGet("addFilmForm")]
        public IActionResult AddFilmForm()
        {
            return View("AddFilm");
        }

        [HttpGet("actors")]
        public IActionResult GetActors()
        {
            return Ok(_serv.Context.Actor.ToList());
        }

        [HttpGet("directors")]
        public IActionResult GetDirectors()
        {
            return Ok(_serv.Context.Director.ToList());
        }

        [HttpPost]
        public async Task AddFilm([FromForm] AddFilmModel model)
        {
            var actors = model.Actors.Split(" ");
            var awards = model.Awards.Split(" ");
            
            Film newFilm = new Film
            {
                DirectorId = model.DirectorId,
                FilmName = model.FilmName,
                Genre = model.Genre,
                PremiereDate = model.PremiereDate
            };
            var addedFilm = await _serv.AddFilm(newFilm);

            foreach( var ac in actors )
            {
                var toAdd = new FilmActors
                {
                    ActorId = Convert.ToInt32(ac),
                    FilmId = addedFilm.Id
                };
                _serv.AddFilmActor(toAdd).Wait();
            }

            foreach(var rew in awards)
            {
                var toAdd = new FilmReward
                {
                    Date = addedFilm.PremiereDate,
                    RewardName = rew,
                    FilmId = addedFilm.Id
                };
                _serv.AddAwards(toAdd).Wait();
            }


            RedirectToAction("AddFilmForm");
        }

        [HttpPut]
        public IActionResult UpdateFilm([FromForm] Film model)
        {
            return Ok(_serv.UpdateFilm(model));
        }

        [HttpGet("search")]
        public IActionResult SearchFilm([FromBody] SearchModel model)
        {
            return Ok(_serv.SearchFilms(model.Name, model.Genre, model.Director, model.Actor));
        }

        [HttpGet("suggets")]
        public IActionResult GetSuggestion()
        {
            return Ok(_serv.GetSuggestion(HttpContext.User.Identity.Name));
        }

        [HttpGet("later/{filmId}")]
        public IActionResult AddWatchLater([FromQuery] int filmId)
        {
            var userID = HttpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name).Value;
            return Ok(_serv.AddWatchLater(Convert.ToInt32(userID), filmId));
        }

        [HttpGet("rate/{filmId}/{rating}")]
        public IActionResult AddWatched([FromQuery] int filmId, [FromQuery] int rating)
        {
            var userID = HttpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name).Value;
            return Ok(_serv.AddWatched(Convert.ToInt32(userID), filmId, rating));
        }

        [HttpGet("bestActor/{date}")]
        public IActionResult GetBestActor([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetBestActor(datetime));
        }

        [HttpGet("activeUser/{date}")]
        public IActionResult GetMostActiveUser([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetMostActiveUser(datetime));
        }

        [HttpGet("ratedActor")]
        public IActionResult GetTopRatedActor()
        {
            return Ok(_serv.GetTopRatedActor());
        }
        [HttpGet("bestDirector/{date}")]
        public IActionResult GetBestDirector([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetBestDirector(datetime));
        }
        [HttpGet("ratedDirector")]
        public IActionResult GetTopRatedDiretcor()
        {
            return Ok(_serv.GetTopRatedDiretcor());
        }
        [HttpGet("ratedFilm")]
        public IActionResult GetTopRatedFilm()
        {
            return Ok(_serv.GetTopRatedFilm());
        }
        [HttpGet("bestFilm/{date}")]
        public IActionResult GetBestFilm([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetBestFilm(datetime));
        }
        [HttpGet("awardedFilm/{date}")]
        public IActionResult GetAwardedFilm([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetBestFilm(datetime));
        }
        [HttpGet("popGenre/{date}")]
        public IActionResult GetMostPopularGenre([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetBestFilm(datetime));
        }
        [HttpGet("topReward/{date}")]
        public IActionResult GetTopReward([FromQuery] string date)
        {
            DateTime? datetime = null;
            if (date != null) datetime = Convert.ToDateTime(date);
            return Ok(_serv.GetBestFilm(datetime));
        }

        private FilmViewModel MapToViewModel(Film film)
        {
            return new FilmViewModel
            {
                FilmName = film.FilmName,
                Genre = film.Genre,
                Director = film.Director.DirectorName,
                PremiereDate = film.PremiereDate.Value.Date.ToString(),
                Rating = film.Rating.HasValue ? film.Rating.Value : 0,
                Actors = _serv.GetFilmActors().Where(f => f.FilmId == film.Id).Select(a => a.Actor.ActorName).ToArray(),
                Awards = _serv.GetFilmRewards().Where(r => r.FilmId == film.Id).Select(w => w.RewardName).ToArray()
            };
        }
    }
}