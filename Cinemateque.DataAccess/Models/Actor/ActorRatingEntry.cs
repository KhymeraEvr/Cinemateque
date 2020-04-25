using System;
using System.Collections.Generic;
using System.Text;

namespace Cinemateque.DataAccess.Models
{
   public class ActorRatingEntry
   {
      public int Id { get; set; }
      public Actor Actor { get; set; }
      public DateTime Date { get; set; }
      public double? Rating { get; set; }
   }
}
