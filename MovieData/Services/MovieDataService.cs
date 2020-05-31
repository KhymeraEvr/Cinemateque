using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cinemateque.DataAccess;
using Cinemateque.DataAccess.Models;
using Cinemateque.DataAccess.Models.Movie;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using MoviesProcessing.Models;

namespace MovieData.Services
{
   public class MovieDataService : IMovieDataService
   {
      private readonly CinematequeContext _context;
      private readonly IRatingAnalizer _ratingAnalizer;

      public MovieDataService(CinematequeContext context, IRatingAnalizer ratingAnalizer)
      {
         _context = context;
         _ratingAnalizer = ratingAnalizer;
      }

      public async Task<string> GetActorCsv(string actorName, CastModel actorModel = null)
      {
         var actor = _context.Actors.Include(ac => ac.Ratings).FirstOrDefault(x => x.ActorName == actorName);

         if (actor == null)
         {
            actor = await WhereIsThatOldNastyDog(actorModel);
         };

         var csvData = actor.Ratings.Select(x => new
         {
            Rating = x.Rating,
            Date = x.Date.ToString("d")
         });

         var fileName = GetValidFileName(actorName);
         var fileDir = $"..\\ActorsCsvs\\{fileName}.csv";

         using (var writer = new StreamWriter(fileDir))
         using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
         {
            csv.WriteRecords(csvData);
         }

         return fileDir;
      }

      public async Task<string> GetCrewCsv(string crewName, CrewModel crewModel = null)
      {
         var crew = _context.CrewMembers.Include(ac => ac.Ratings).FirstOrDefault(x => x.Name == crewName);

         if (crew == null) crew = await WhereIsThatOldNastyDog(crewModel);

         var csvData = crew.Ratings.Select(x => new
         {
            Rating = x.Rating,
            Date = x.Date.ToString("d")
         });

         var fileName = GetValidFileName(crewName);
         var fileDir = $"..\\CrewCsvs\\{fileName}.csv";

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
         var movie = _context.Movies.FirstOrDefault(x => x.MovieId == id);

         return movie;
      }

      private async Task<Actor> WhereIsThatOldNastyDog(CastModel actorModel)
      {
         if (actorModel == null) throw new KeyNotFoundException();

         var actor = await _ratingAnalizer.AnalizeActor(actorModel);

         if (actor.Ratings.Count >= 10)
         {
            var updateResult = await _context.Actors.AddAsync(actor);
         }

         await _context.SaveChangesAsync();

         return actor;
      }

      private async Task<CrewMember> WhereIsThatOldNastyDog(CrewModel actorModel)
      {
         if (actorModel == null) throw new KeyNotFoundException();

         var actor = await _ratingAnalizer.AnalizeCrew(actorModel);

         return actor;
      }

      private string GetValidFileName(string fileName)
      {
         // remove any invalid character from the filename.
         String ret = Regex.Replace(fileName.Trim(), "[^A-Za-z0-9_. ]+", "");
         return ret.Replace(" ", String.Empty);
      }
   }
}
