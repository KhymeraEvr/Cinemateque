using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.DataAccess.Models.Movie;
using MoviesProcessing.Models;
using MoviesProcessing.Services;

namespace MovieData.Services
{
   public class RatingPredictionService
   {
      private const int TakeProducers = 2;
      private const int TakeCrew = 7;

      private readonly HashSet<string> Jobs = new HashSet<string>
      {
         "director", "screenplay", "story", "original music composer"
      };

      private readonly IMovieApiService _movieApiService;
      private readonly IMovieDataService _movieDataService;
      private readonly IRatingAnalizer _ratingAnalizer;

      public RatingPredictionService(
         IMovieApiService movieApiService,
         IMovieDataService movieDataService,
         IRatingAnalizer ratingAnalizer)
      {
         _movieApiService = movieApiService;
         _movieDataService = movieDataService;
         _ratingAnalizer = ratingAnalizer;
      }

      public async Task GetMovieRatingPrediction(int movieId)
      {
         var dataModel = await GetDataModel(movieId);
      }

      public async Task<MovieDataModel> GetDataModel(int movieId)
      {
         var model = _movieDataService.GetMovie(movieId);
         if (model != null) return model;

         var idStr = movieId.ToString();

         var movieDetails = await _movieApiService.GetMovieDetails(idStr);
         var movieCast = await _movieApiService.GetCredits(idStr);

         var top10Actors = movieCast.Cast.Take(10);
         var actorsCsvs = await GetActorsCsvs(top10Actors);
         var actorsPopularity = await GetActorsPopularity(top10Actors);

         var crew = FilterCrew(movieCast.Crew);
         var crewMembersCsvs = await GetCrewCsvs(crew);
         var crewPopularity = await GetCrewPopularity(crew);

         var movieGenres = movieDetails.Genres.Select(x => x.Name);

         var budget = movieDetails.Budget;

         var companies = movieDetails.Companies.Select(x => x.Name);

         var movieDataModel = new MovieDataModel
         {
            Id = movieId,
            ActorsCsvs = actorsCsvs.ToList(),
            ActorsPopularity = actorsPopularity.ToList(),
            CrewCsvs = crewMembersCsvs.ToList(),
            CrewPopularity = crewPopularity.ToList(),
            Genres = movieGenres.ToList(),
            Budget = budget,
            Companies = companies.ToList()
         };

         await _movieDataService.SaveMovie(movieDataModel);

         return movieDataModel;
      }

      private async Task<IEnumerable<string>> GetActorsCsvs(IEnumerable<CastModel> actors)
      {
         var results = new List<string>();

         foreach (var actor in actors)
         {
            var dir = await _movieDataService.GetActorCsv(actor.Name);
            results.Add(dir);
         }

         return results;
      }

      private async Task<IEnumerable<string>> GetCrewCsvs(IEnumerable<CrewModel> crews)
      {
         foreach (var crew in crews)
         {
            await _ratingAnalizer.AnalizeCrew(crew);
         }

         var results = new List<string>();

         foreach (var actor in crews)
         {
            var dir = await _movieDataService.GetCrewCsv(actor.Name);
            results.Add(dir);
         }

         return results;
      }

      private IEnumerable<CrewModel> FilterCrew(IEnumerable<CrewModel> crews)
      {
         var crew = crews.Where(x => Jobs.Contains(x.Job.ToLower()));
         var producers = crew.Where(x => x.Job.ToLower() == "Producer").Take(TakeProducers);

         var result = crew.Concat(producers).OrderBy(x => x.Job).Take(TakeCrew);

         return result;
      }

      private async Task<IEnumerable<double>> GetActorsPopularity(IEnumerable<CastModel> actors)
      {
         var results = new List<double>();

         foreach (var actor in actors)
         {
            var details = await _movieApiService.GetPersonDetails(actor.Id.ToString());
            results.Add(details.Popularity);
         }

         return results;
      }

      private async Task<IEnumerable<double>> GetCrewPopularity(IEnumerable<CrewModel> crews)
      {
         var results = new List<double>();

         foreach (var crew in crews)
         {
            var details = await _movieApiService.GetPersonDetails(crew.Id.ToString());
            results.Add(details.Popularity);
         }

         return results;
      }
   }
}
