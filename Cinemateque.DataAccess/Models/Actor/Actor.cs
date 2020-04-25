using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
   public class Actor
   {
      public int Id { get; set; }
      public string ActorName { get; set; }
      public ICollection<ActorRatingEntry> Ratings { get; set; }
   }
}
