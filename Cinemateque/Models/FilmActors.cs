using System;
using System.Collections.Generic;

namespace Cinemateque.Models
{
    public partial class FilmActors
    {
        public int Id { get; set; }
        public int? ActorId { get; set; }
        public int? FilmId { get; set; }

        public Actor Actor { get; set; }
        public Film Film { get; set; }
    }
}
