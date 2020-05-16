using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models.Movie
{
   public class MovieDataModel
   {
      public int Id { get; set; }

      public ICollection<string> ActorsCsvs { get; set; }

      public ICollection<double> ActorsPopularity { get; set; }

      public ICollection<string> CrewCsvs { get; set; }

      public ICollection<double> CrewPopularity { get; set; }

      public ICollection<string> Genres { get; set; }

      public double Budget { get; set; }
      
      public ICollection<string> Companies { get; set; }
   }
}
