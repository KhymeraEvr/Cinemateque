using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class UserFilms
    {
        public int? UserId { get; set; }
        public int? FilmId { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime? Time { get; set; }
        public float? Rating { get; set; }

        public Film Film { get; set; }
        public User User { get; set; }
    }
}
