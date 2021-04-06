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
        [Display(Name = "Athletic Ability")]
        public string AthleticAbility { get; set; }
        [Display(Name = "Skill Level")]
        public string SkillLevel { get; set; }
        [Display(Name = "Scheduled Time")]
        public DateTime? ScheduledTIme { get; set; }
        public string Accessibility { get; set; }
        public string LocationsName { get; set; }
        public double? LocationsLatitude { get; set; }
        public double? LocationsLongitude { get; set; }
        [Display(Name = "Number of Attendees")]
        public int AttendanceCount { get; set; }

        [ForeignKey("AthleteId")]
        public int HostAthleteId { get; set; }
        public Athlete HostAthlete { get; set; }

        [ForeignKey("LocationId")]
        public int LocationId { get; set; }
        public Location Location { get; set; }


    }
}
