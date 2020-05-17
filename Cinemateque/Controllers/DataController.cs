using System.Linq;
using System.Threading.Tasks;
using Cinemateque.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieData.Services;
using MoviesProcessing.Services;

namespace Cinemateque.Controllers
{

   [Route("api/[controller]")]
   [ApiController]
   public class DataController : ControllerBase
   {
      private readonly CinematequeContext _context;
      private readonly IRatingAnalizer _analizer;
      private readonly IMovieApiService _movieApiService;

      public DataController(
         CinematequeContext context,
         IRatingAnalizer analizer,
         IMovieApiService movieApiService)
      {
         _context = context;
         _analizer = analizer;
         _movieApiService = movieApiService;
      }

      [HttpGet("actors")]
      public async Task<IActionResult> GetActors()
      {
         var actors5 = _context.Actors.Where(x => x.Ratings.Count < 5).Count();
         var actors10 = _context.Actors.Where(x => x.Ratings.Count < 10).Count();
         var actors15 = _context.Actors.Where(x => x.Ratings.Count < 20).Count();
         var actors20 = _context.Actors.Where(x => x.Ratings.Count > 20).Count();


         return Ok(actors5);
      }

      [HttpGet("companies")]
      public async Task<IActionResult> GetCompanies()
      {
         var companies = await _analizer.GetProductionCompanies();

         return Ok(companies);
      }

      [HttpGet("genres")]
      public async Task<IActionResult> GetGenres()
      {
         var gernres = await _movieApiService.GetGenres();
         var list = gernres.Select(x => x.Name);

         return Ok(list);
      }

      [HttpGet("actorsCleanUp")]
      public async Task<IActionResult> CleanupActors()
      {
         var actors = _context.Actors.Include(x => x.Ratings).Where(x => x.Ratings.Count < 10).ToList();
         foreach (var ac in actors)
         {
            if (ac.Ratings == null) continue;
            _context.RemoveRange(ac.Ratings);

         }

         await _context.SaveChangesAsync();

         _context.RemoveRange(actors);

         await _context.SaveChangesAsync();

         return Ok();
      }
   }
}
