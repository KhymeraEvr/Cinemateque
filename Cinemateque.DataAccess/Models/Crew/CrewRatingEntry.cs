using System;

namespace Cinemateque.DataAccess.Models.Crew
{
   public class CrewRatingEntry
   {
      public int Id { get; set; }
      public CrewMember CrewMember { get; set; }
      public DateTime Date { get; set; }
      public double? Rating { get; set; }
   }
}
