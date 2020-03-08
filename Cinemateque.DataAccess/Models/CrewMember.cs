using System;
using System.Collections.Generic;
using System.Text;

namespace Cinemateque.DataAccess.Models
{
   public class CrewMember
   {
      public int Id { get; set; }
      public string Job { get; set; }
      public string Name { get; set; }
      public double? Rating { get; set; }
      public int FilmsChecked { get; set; }

   }
}
