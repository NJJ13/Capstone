using FreshAir.Data;
using FreshAir.Models;
using FreshAir.Services;
using FreshAir.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FreshAir.Controllers
{
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeocodeServiceAthlete _geocodingServiceAthlete;
        private readonly GeocodeServiceLocation _geocodingServiceLocation;
        private readonly DistanceMatrixService _distanceMatrixService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocationController(ApplicationDbContext context, GeocodeServiceAthlete athleteGeocode, GeocodeServiceLocation locationGeocode, DistanceMatrixService distanceMatrixService,
                                    IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _geocodingServiceAthlete = athleteGeocode;
            _geocodingServiceLocation = locationGeocode;
            _distanceMatrixService = distanceMatrixService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: LocationController
        public ActionResult Index()
        {
            var locations = _context.Locations;
            return View(locations);
        }

        // GET: LocationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LocationController/Create
        public ActionResult CreateLocation()
        {
            return View();
        }

        // POST: LocationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(LocationViewModel locationVM)
        {
            
            string stringFileName = ProcessLocationFile(locationVM);
            var location = new Location
            {
                LocationName = locationVM.LocationName,
                Address = locationVM.Address,
                City = locationVM.City,
                State = locationVM.State,
                ZipCode = locationVM.ZipCode,
                Description = locationVM.Description,
                Picture = stringFileName
            };
            var locationWithLatandLong = await _geocodingServiceLocation.GetGeocoding(location);

            if (locationWithLatandLong.Picture == null)
            {
                locationWithLatandLong.Picture = "no-image.jpg";
            }
            _context.Locations.Add(locationWithLatandLong);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Athlete");

        }

        // GET: LocationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LocationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LocationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LocationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        private string ProcessLocationFile(LocationViewModel locationVM)
        {
            string newFileName = null;
            if (locationVM.Picture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                newFileName = Guid.NewGuid().ToString() + "_" + locationVM.Picture.FileName;
                string filePath = Path.Combine(uploadsFolder, newFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    locationVM.Picture.CopyTo(fileStream);
                }
            }
            return newFileName;
        }
    }
}
