using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string Activity { get; set; }
        public string Description { get; set; }
        public string AthleticAbility { get; set; }
        public string SkillLevel { get; set; }
        [Display(Name = "Scheduled Time")]
        [DataType(DataType.Date)]
        public DateTime? ScheduledTIme { get; set; }
        public string Accessibility { get; set; }
        public string LocationsName { get; set; }
        public double? LocationsLatitude { get; set; }
        public double? LocationsLongitude { get; set; }
        public int AttendanceCount { get; set; }

        [ForeignKey("AthleteId")]
        public int HostAthleteId { get; set; }
        public Athlete HostAthlete { get; set; }

        [ForeignKey("LocationId")]
        public int LocationId { get; set; }
        public Location Location { get; set; }


    }
}
