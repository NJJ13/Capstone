using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.Models
{
    public class FriendsList
    {
        public int CurrentUserId { get; set; }
        public Athlete CurrentAthlete { get; set; }
        public int FriendId { get; set; }
        public Athlete FriendAthlete { get; set; }
    }
}
