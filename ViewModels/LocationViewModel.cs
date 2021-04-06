using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.ViewModels
{
    public class LocationViewModel
    {
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        public string Description { get; set; }
        [FileExtensions(Extensions = "jpg,jpeg,png,pdf")]
        public IFormFile Picture { get; set; }
        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }
    }
}
