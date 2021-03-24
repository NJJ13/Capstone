using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.Models
{
    public class Athlete
    {
        [Key]
        public int AthleteId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string FirstInterest { get; set; }
        public string SecondInterest { get; set; }
        public string ThirdInterest { get; set; }
        public string AthleticAbility { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string LikedAthletes { get; set; }
        public double? DistanceModifier { get; set; }
        public double? AthleteLatitude { get; set; }
        public double? AthleteLongitude { get; set; }


        [ForeignKey("IdentityUser")]
        public string IdentityUserId { get; set; }
        public IdentityUser IdentityUser { get; set; }

    }
}
