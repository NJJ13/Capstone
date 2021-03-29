using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.Models
{
    public class AthleteEvent
    {
        //[ForeignKey ("Athlete")]
        public int? AthleteId { get; set; }
        public Athlete Athlete { get; set; }
        //[ForeignKey("Event")]
        public int? EventId { get; set; }
        public Event Event { get; set; }
    }
}
