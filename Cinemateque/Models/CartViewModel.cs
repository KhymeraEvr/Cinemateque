using Cinemateque.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemateque.Models
{
    public struct CartViewModel
    {
        public ICollection<Order> Orders { get; set; }
        public double TotalSum { get; set; }
    }
}
