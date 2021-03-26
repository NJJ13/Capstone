using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.ViewModels
{
    public class AthleteViewModel
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string FirstInterest { get; set; }
        public string SecondInterest { get; set; }
        public string ThirdInterest { get; set; }
        public string AthleticAbility { get; set; }
        [FileExtensions(Extensions = "jpg,jpeg,png,pdf")]
        public IFormFile ProfilePicture { get; set; }
        public string LikedAthletes { get; set; }
        public double? DistanceModifier { get; set; }
        public double? AthleteLatitude { get; set; }
        public double? AthleteLongitude { get; set; }
    }
}
