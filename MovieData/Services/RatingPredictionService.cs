using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

      public async Task GetDiscoverPageData(int page)
      {
         var movies = await _movieApiService.GetDiscoverFilms(page);
         foreach (var movie in movies)
         {
            try
            {
               await GetMovieRatingPrediction(movie.Id);
            }
            catch (Exception e)
            {
               using (System.IO.StreamWriter file =
          new System.IO.StreamWriter("analyze.log", true))
               {
                  file.WriteLine(e.Message);
               }
            }
            System.Threading.Thread.Sleep(5000);
         }
      }

      public async Task GetTopPageData(int page)
      {
         var movies = await _movieApiService.GetTopRatedMoves(page);
         foreach (var movie in movies)
         {
            await GetMovieRatingPrediction(movie.Id);
            System.Threading.Thread.Sleep(5000);
         }
      }

      public async Task GetMovieRatingPrediction(int movieId)
      {
         var dataModel = await GetDataModel(movieId);
         var fileName = GetValidFileName(dataModel.Title);
         var fileDir = $"..\\MoviesCsvs\\{fileName}.csv";

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


         var actors = top10Actors.Select(x => x.Name).ToList();
         var actorsPop = actorsPopularity.ToList();
         var crews = crew.Select(x => x.Name).ToList();
         var crewPop = crewPopularity.ToList();
         var budget = movieDetails.Budget;

         var companies = movieDetails.Companies.Select(x => x.Name);
         var companiesFlags = GetCompaniesBools(companies);

         var movieDataModel = new MovieDataModel
         {
            MovieId = movieId,
            Title = movieDetails.Title,
            ActorsCsvs = GetEnough(actors, 10).ToArray(),
            ActorsPopularity = GetEnough(actorsPop, 10).ToArray(),
            CrewCsvs = GetEnough(crews, TakeCrew).ToArray(),
            CrewPopularity = GetEnough(crewPop, TakeCrew).ToArray(),
            Genres = movieGenres.ToArray(),
            Budget = budget,
            Companies = companies.ToArray(),
            GenresFlags = genresFlags.ToArray(),
            CompaniesFlags = companiesFlags.ToArray(),
            ReleaseDate = movieDetails.ReleaseDate,
            Rating = movieDetails.VoteAverage
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
            var dir = await _movieDataService.GetActorCsv(actor.Name, actor);
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
            var dir = await _movieDataService.GetCrewCsv(actor.Name, actor);
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

      private List<T> GetEnough<T>(List<T> source, int amount)
      {
         if (source.Count() == 0) return new List<T>();

         var list = source.Take(amount).ToList();
         if (list.Count != amount)
         {
            while (list.Count != amount)
            {
               list.Add(source[0]);
            }
         }

         return list;
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
            Companies = entity.Companies.Split(';'),
            ReleaseDate = entity.ReleaseDate,
            Rating = entity.Rating
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
            Companies = string.Join(';', model.Companies),
            CompaniesFlags = string.Join(';', model.CompaniesFlags),
            GenresFlags = string.Join(';', model.GenresFlags),
            ReleaseDate = model.ReleaseDate,
            Rating = model.Rating
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

      private string GetValidFileName(string fileName)
      {
         // remove any invalid character from the filename.
         String ret = Regex.Replace(fileName.Trim(), "[^A-Za-z0-9_. ]+", "");
         return ret.Replace(" ", String.Empty);
      }
   }
}
