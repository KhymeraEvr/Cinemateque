using Cinemateque.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinemateque.Models
{
    public class EditViewModel
    {
        public ICollection<Order> Oreders { get; set; }
        public string User { get; set; }
    }
}
