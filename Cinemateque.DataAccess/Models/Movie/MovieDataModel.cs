namespace Cinemateque.DataAccess.Models.Movie
{
   public class MovieDataModel
   {
      public int MovieId { get; set; }

      public string Title { get; set; }

      public string[] ActorsCsvs { get; set; }

      public double[] ActorsPopularity { get; set; }

      public string[] CrewCsvs { get; set; }

      public double[] CrewPopularity { get; set; }

      public string[] Genres { get; set; }

      public int[] GenresFlags { get; set; }

      public double Budget { get; set; }

      public string[] Companies { get; set; }

      public int[] CompaniesFlags { get; set; }
   }
}
