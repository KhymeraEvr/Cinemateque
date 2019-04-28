﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemateque.Models
{
    public class FilmViewModel
    {
        public string FilmName { get; set; }
        public string Genre { get; set; }
        public string PremiereDate { get; set; }
        public string Director { get; set; }
        public int Rating { get; set; }
        public string[] Actors { get; set; }
        public string[] Awards { get; set; }
    }
}