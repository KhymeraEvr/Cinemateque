using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class Director
    {
        public Director()
        {
            Film = new HashSet<Film>();
        }

        public int Id { get; set; }
        public string DirectorName { get; set; }
        public int? Rating { get; set; }

        public ICollection<Film> Film { get; set; }
    }
}
