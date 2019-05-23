using System;

namespace Cinemateque.Models
{
    public class AddFilmModel
    {
        public string FilmName { get; set; }
        public string Genre { get; set; }
        public DateTime? PremiereDate { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Image { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
    }
}
