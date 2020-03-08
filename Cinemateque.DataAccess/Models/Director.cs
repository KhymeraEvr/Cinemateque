﻿using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
   public partial class Director
   {
      public int Id { get; set; }
      public string DirectorName { get; set; }
      public double? Rating { get; set; }

      public int FilmsChecked { get; set; }

   }
}
