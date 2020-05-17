using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.DataAccess.Models.Movie;
using CsvHelper;
using MoviesProcessing.Models;
using MoviesProcessing.Services;

namespace MovieData.Services
{
   public class RatingPredictionService : IRatingPredictionService
   {
      private const int TakeProducers = 2;
      private const int TakeCrew = 7;

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

         var fileDir = $"..\\MoviesCsvs\\{dataModel.Title}.csv";

         using (var writer = new StreamWriter(fileDir))
         using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
         {
            csv.WriteRecord(dataModel);
         }
      }

      public async Task<MovieDataEntity> GetDataModel(int movieId)
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
         var genresFlags = GetGenresBools(movieGenres);


         var budget = movieDetails.Budget;

         var companies = movieDetails.Companies.Select(x => x.Name);
         var companiesFlags = GetCompaniesBools(companies);

         var movieDataModel = new MovieDataModel
         {
            MovieId = movieId,
            Title = movieDetails.Title,
            ActorsCsvs = actorsCsvs.ToArray(),
            ActorsPopularity = actorsPopularity.ToArray(),
            CrewCsvs = crewMembersCsvs.ToArray(),
            CrewPopularity = crewPopularity.ToArray(),
            Genres = movieGenres.ToArray(),
            Budget = budget,
            Companies = companies.ToArray(),
            GenresFlags = genresFlags.ToArray(),
            CompaniesFlags = companiesFlags.ToArray()
         };

         var movieDataEntity = Map(movieDataModel);

         await _movieDataService.SaveMovie(movieDataEntity);

         return movieDataEntity;
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
         var crew = crews.Where(x => ListsProvider.Jobs.Contains(x.Job.ToLower()));
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

      private MovieDataModel Map(MovieDataEntity entity)
      {
         var model = new MovieDataModel
         {
            MovieId = entity.MovieId,
            Title = entity.Title,
            ActorsCsvs = entity.ActorsCsvs.Split(';'),
            ActorsPopularity = entity.ActorsPopularity.Split(';').Select(x => double.Parse(x)).ToArray(),
            CrewCsvs = entity.CrewCsvs.Split(';'),
            CrewPopularity = entity.CrewPopularity.Split(';').Select(x => double.Parse(x)).ToArray(),
            Genres = entity.Genres.Split(';'),
            Budget = entity.Budget,
            Companies = entity.Companies.Split(';')
         };

         return model;
      }

      private MovieDataEntity Map(MovieDataModel model)
      {
         var entity = new MovieDataEntity
         {
            MovieId = model.MovieId,
            Title = model.Title,
            ActorsCsvs = string.Join(';', model.ActorsCsvs),
            ActorsPopularity = string.Join(';', model.ActorsPopularity),
            CrewCsvs = string.Join(';', model.CrewCsvs),
            CrewPopularity = string.Join(';', model.CrewPopularity),
            Genres = string.Join(';', model.Genres),
            Budget = model.Budget,
            Companies = string.Join(';', model.Companies)
         };

         return entity;
      }

      private IEnumerable<int> GetGenresBools(IEnumerable<string> genres)
      {
         var n = ListsProvider.Genres.Count;
         var res = new int[n];

         for (int i = 0; i < n; i++)
         {
            res[i] = genres.Contains(ListsProvider.Genres[i]) ? 1 : 0;
         }

         return res;
      }

      private IEnumerable<int> GetCompaniesBools(IEnumerable<string> companies)
      {
         var n = ListsProvider.Companies.Count;
         var res = new int[n];

         for (int i = 0; i < n; i++)
         {
            res[i] = companies.Contains(ListsProvider.Companies[i]) ? 1 : 0;
         }

         return res;
      }
   }
}
