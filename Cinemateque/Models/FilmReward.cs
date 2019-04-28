using System;
using System.Collections.Generic;

namespace Cinemateque.Models
{
    public partial class FilmReward
    {
        public int Id { get; set; }
        public int? FilmId { get; set; }
        public DateTime? Date { get; set; }
        public string RewardName { get; set; }

        public Film Film { get; set; }
    }
}
