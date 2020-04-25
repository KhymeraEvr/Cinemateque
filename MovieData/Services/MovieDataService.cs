using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinemateque.DataAccess;
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

      public async Task GetActorCsv(string actorName)
      {
         var actor = _context.Actors.Include(ac => ac.Ratings).FirstOrDefault(x => x.ActorName == actorName);

         if (actor == null) throw new KeyNotFoundException();
         var actorData = actor.Ratings;

         var csvData = actor.Ratings.Select(x => new
         {
            Rating = x.Rating,
            Date = x.Date.ToString("d")
         });

         using (var writer = new StreamWriter($"..\\{actorName}.csv"))
         using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
         {
            csv.WriteRecords(csvData);
         }

      }
   }
}
