using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.DataAccess;
using Cinemateque.DataAccess.Models.Movie;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace MovieData.Services
{
   public class MovieDataService : IMovieDataService
   {
      private readonly CinematequeContext _context;

      public MovieDataService(CinematequeContext context)
      {
         _context = context;
      }

      public async Task<string> GetActorCsv(string actorName)
      {
         var actor = _context.Actors.Include(ac => ac.Ratings).FirstOrDefault(x => x.ActorName == actorName);

         if (actor == null) throw new KeyNotFoundException();

         var csvData = actor.Ratings.Select(x => new
         {
            Rating = x.Rating,
            Date = x.Date.ToString("d")
         });

         var fileDir = $"..\\ActorsCsvs\\{actorName}.csv";

         using (var writer = new StreamWriter(fileDir))
         using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
         {
            csv.WriteRecords(csvData);
         }

         return fileDir;
      }

      public async Task<string> GetCrewCsv(string crewName)
      {
         var crew = _context.CrewMembers.Include(ac => ac.Ratings).FirstOrDefault(x => x.Name == crewName);

         if (crew == null) throw new KeyNotFoundException();

         var csvData = crew.Ratings.Select(x => new
         {
            Rating = x.Rating,
            Date = x.Date.ToString("d")
         });

         var fileDir = $"..\\CrewCsvs\\{crewName}.csv";

         using (var writer = new StreamWriter(fileDir))
         using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
         {
            csv.WriteRecords(csvData);
         }

         return fileDir;
      }

      public async Task SaveMovie(MovieDataEntity movie)
      {
         await _context.Movies.AddAsync(movie);
         await _context.SaveChangesAsync();
      }

      public MovieDataEntity GetMovie(int id)
      {
         var movie  = _context.Movies.FirstOrDefault( x => x.MovieId == id);

         return movie;
      }
   }
}
