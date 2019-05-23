using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int FilmId { get; set; }
        public string Status { get; set; }

        public Film Film { get; set; }
        public User User { get; set; }
    }
}
