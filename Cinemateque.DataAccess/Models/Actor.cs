using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class Actor
    {
        public Actor()
        {
            FilmActors = new HashSet<FilmActors>();
        }

        public int Id { get; set; }
        public string ActorName { get; set; }
        public int? Rating { get; set; }

        public ICollection<FilmActors> FilmActors { get; set; }
    }
}
