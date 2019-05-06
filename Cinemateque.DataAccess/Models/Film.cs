using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class Film
    {
        public Film()
        {
            FilmActors = new HashSet<FilmActors>();
            FilmReward = new HashSet<FilmReward>();
            UserFilms = new HashSet<UserFilms>();
        }

        public string FilmName { get; set; }
        public string Genre { get; set; }
        public DateTime? PremiereDate { get; set; }
        public int? DirectorId { get; set; }
        public int Id { get; set; }
        public int? Rating { get; set; }

        public Director Director { get; set; }
        public ICollection<FilmActors> FilmActors { get; set; }
        public ICollection<FilmReward> FilmReward { get; set; }
        public ICollection<UserFilms> UserFilms { get; set; }
    }
}
