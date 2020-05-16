namespace Cinemateque.DataAccess.Models.Movie
{
   public class MovieDataEntity
   {
      public int Id { get; set; }

      public int MovieId { get; set; }

      public string Title { get; set; }

      public string ActorsCsvs { get; set; }

      public string ActorsPopularity { get; set; }

      public string CrewCsvs { get; set; }

      public string CrewPopularity { get; set; }

      public string Genres { get; set; }

      public double Budget { get; set; }

      public string Companies { get; set; }
   }
}
