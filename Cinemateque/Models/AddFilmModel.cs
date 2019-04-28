using System;

namespace Cinemateque.Models
{
    public class AddFilmModel
    {
        public string FilmName { get; set; }
        public string Genre { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? DirectorId { get; set; }
        public string Actors { get; set; }
        public string Awards { get; set; }
    }
}
