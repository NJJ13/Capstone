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
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string State { get; set; }
        [Display(Name = "Zip Code")]
        [StringLength(10, MinimumLength = 5)]
        public string ZipCode { get; set; }
        [Display(Name = "Primary Interest")]
        public string FirstInterest { get; set; }
        [Display(Name = "Secondary Interest")]
        public string SecondInterest { get; set; }
        [Display(Name = "Tertiary Interest")]
        public string ThirdInterest { get; set; }
        [Display(Name = "Athletic Ability")]
        public string AthleticAbility { get; set; }
        [Display(Name = "Profile Picture")]
        [FileExtensions(Extensions = "jpg,jpeg,png,pdf")]
        public IFormFile ProfilePicture { get; set; }
        [Display(Name = "Distance Willing to Travel:")]
        public double? DistanceModifier { get; set; }
        public double? AthleteLatitude { get; set; }
        public double? AthleteLongitude { get; set; }
    }
}
