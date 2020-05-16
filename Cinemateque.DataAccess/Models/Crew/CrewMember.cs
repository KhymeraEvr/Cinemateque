using System.Collections.Generic;
using Cinemateque.DataAccess.Models.Crew;

namespace Cinemateque.DataAccess.Models
{
   public class CrewMember
   {
      public int Id { get; set; }
      public string Job { get; set; }
      public string Name { get; set; }
      public ICollection<CrewRatingEntry> Ratings { get; set; }
      public int FilmsChecked { get; set; }

   }
}
