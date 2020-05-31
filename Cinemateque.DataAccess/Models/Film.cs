using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class Film
    {
        public Film()
        {
            UserFilms = new HashSet<UserFilms>();
        }

        public string FilmName { get; set; }
        public string Genres { get; set; }
        public string PremiereDate { get; set; }
        public string Director { get; set; }
        public int Id { get; set; }
        public float? Rating { get; set; }
        public string Actors { get; set; }
         public int Movieid { get; set; }

        public ICollection<UserFilms> UserFilms { get; set; }
    }
}
