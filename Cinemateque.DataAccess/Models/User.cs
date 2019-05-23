using System;
using System.Collections.Generic;

namespace Cinemateque.DataAccess.Models
{
    public partial class User
    {
        public User()
        {
            Order = new HashSet<Order>();
            UserFilms = new HashSet<UserFilms>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Passwrod { get; set; }
        public int? Rating { get; set; }
        public string Token { get; set; }

        public ICollection<Order> Order { get; set; }
        public ICollection<UserFilms> UserFilms { get; set; }
    }
}
