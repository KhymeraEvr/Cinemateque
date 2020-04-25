﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieData.Services;
using MoviesProcessing.Services;

namespace Cinemateque.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AnalyzeController : ControllerBase
   {
      private readonly IMovieApiService _movieApiService;
      private readonly IRatingAnalizer _analizer;
      private readonly IMovieDataService _dataService;

      public AnalyzeController(
         IMovieApiService movieApiService,
        IRatingAnalizer analizer,
        IMovieDataService dataService)
      {
         _movieApiService = movieApiService;
         _analizer = analizer;
         _dataService = dataService;
      }

      [HttpGet("actors/{page}")]
      public async Task AnalizeActorPage(int page)
      {
         var topMovies = await _movieApiService.GetTopRatedMoves(page);
         foreach (var movie in topMovies)
         {
            var credits = await _movieApiService.GetCredits(movie.Id.ToString());
            var cast = credits.Cast;

            await _analizer.AnalizeActors(cast);
         }
      }

      [HttpGet("actors/data/{actorName}")]
      public async Task GetActorData(string actorName)
      {
         await _dataService.GetActorCsv(actorName);
      }
   }
}