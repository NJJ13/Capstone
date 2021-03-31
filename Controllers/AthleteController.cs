using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FreshAir.Data;
using FreshAir.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FreshAir.Services;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using FreshAir.ViewModels;

namespace FreshAir.Controllers
{
    public class AthleteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GeocodeServiceAthlete _geocodingServiceAthlete;
        private readonly GeocodeServiceLocation _geocodingServiceLocation;
        private readonly DistanceMatrixService _distanceMatrixService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AthleteController(ApplicationDbContext context, GeocodeServiceAthlete athleteGeocode, GeocodeServiceLocation locationGeocode, DistanceMatrixService distanceMatrixService,
                                    IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _geocodingServiceAthlete = athleteGeocode;
            _geocodingServiceLocation = locationGeocode;
            _distanceMatrixService = distanceMatrixService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Athlete
        public IActionResult Index()
        {
            var athlete = GetCurrentUser();
            if (athlete == null)
            {
                return RedirectToAction("CreateVM");
            }
            var view = _context.Athletes.Where(a => a.AthleteId == athlete.AthleteId).ToList();
            return View(view);
        }

        // GET: Athlete/Details/5
        public IActionResult MyDetails()
        {
            var athlete = GetCurrentUser();
            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await _context.Athletes.Include(a => a.IdentityUser).FirstOrDefaultAsync(m => m.AthleteId == id);
            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }
        public IActionResult CreateVM()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateVM(AthleteViewModel athleteVM)
        {
            string stringFileName = ProcessUploadedFile(athleteVM);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var athlete = new Athlete
            {
                FirstName = athleteVM.FirstName,
                LastName = athleteVM.LastName,
                State = athleteVM.State,
                ZipCode = athleteVM.ZipCode,
                FirstInterest = athleteVM.FirstInterest,
                SecondInterest = athleteVM.SecondInterest,
                ThirdInterest = athleteVM.ThirdInterest,
                AthleticAbility = athleteVM.AthleticAbility,
                ProfilePicture = stringFileName
            };
            athlete.IdentityUserId = userId;
            var athleteWithLatandLong = await _geocodingServiceAthlete.GetGeocoding(athlete);
            athleteWithLatandLong.DistanceModifier = 5;

            if(athleteWithLatandLong.ProfilePicture == null)
            {
                athleteWithLatandLong.ProfilePicture = "anon.png";
            }
            _context.Athletes.Add(athleteWithLatandLong);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Athlete/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Athlete/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Athlete athlete)
        {
            if (ModelState.IsValid)
            {
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                athlete.IdentityUserId = userId;

                var athleteWithLatandLong = await _geocodingServiceAthlete.GetGeocoding(athlete);
                athleteWithLatandLong.DistanceModifier = 20;

                athleteWithLatandLong.ProfilePicture = "anon.png";
                
                _context.Add(athleteWithLatandLong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", athlete.IdentityUserId);
            return View(athlete);
        }

        public IActionResult CreateEvent(int id)
        {
            var location = _context.Locations.Find(id);
            ViewBag.LocationPicture = location.Picture;
            ViewBag.LocationInfo = location.Description;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(int id, Event newEvent)
        {
            if (ModelState.IsValid)
            {
                var location = _context.Locations.Find(id);
                newEvent.LocationsLatitude = location.LocationLatitude;
                newEvent.LocationsLongitude = location.LocationLongitude;
                newEvent.Location = location;
                newEvent.LocationId = location.LocationId;
                var applicationDbContext = _context.Athletes.Include(a => a.IdentityUser);
                var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var athlete = _context.Athletes.Where(o => o.IdentityUserId == userId).FirstOrDefault();
                newEvent.HostAthlete = athlete;
                newEvent.HostAthleteId = athlete.AthleteId;

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult EventDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theEvent = _context.Events.Find(id);
            if (theEvent == null)
            {
                return NotFound();
            }
            var hostAthlete = _context.Athletes.Find(theEvent.HostAthleteId);
            var location = _context.Locations.Find(theEvent.LocationId);
            ViewBag.HostPicture = hostAthlete.ProfilePicture;
            ViewBag.HostAthleteName = (hostAthlete.FirstName + " " + hostAthlete.LastName);
            ViewBag.LocationPicture = location.Picture;
            ViewBag.LocationDescription = location.Description;
            return View(theEvent);
        }
        public async Task<IActionResult> AttendEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var attendingAthlete = GetCurrentUser();
            var eventToAttend = _context.Events.Find(id);
            if(eventToAttend == null)
            {
                return NotFound();
            }
            var athleteEvent = new AthleteEvent
            {
                Athlete = attendingAthlete,
                AthleteId = attendingAthlete.AthleteId,
                Event = eventToAttend,
                EventId = eventToAttend.EventId
            };
            attendingAthlete.Events.Add(athleteEvent);
            eventToAttend.Attendees.Add(athleteEvent);
            _context.Update(attendingAthlete);
            _context.Update(eventToAttend);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Event = await _context.Events.FindAsync(id);
            var location = _context.Locations.Find(Event.LocationId);
            ViewBag.LocationPicture = location.Picture;
            ViewBag.LocationInfo = location.Description;
            if (Event == null)
            {
                return NotFound();
            }
            return View(Event);
        }

        // POST: Athlete/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int? id, Event Event)
        {
            if (id != Event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var eventToUpdate = _context.Events.Find(id);
                try
                {

                    eventToUpdate.ScheduledTIme = Event.ScheduledTIme;
                    eventToUpdate.Activity = Event.Activity;
                    eventToUpdate.AthleticAbility = Event.AthleticAbility;
                    eventToUpdate.Accessibility = Event.Accessibility;
                    eventToUpdate.Description = Event.Description;
                    eventToUpdate.SkillLevel = Event.SkillLevel;
                    _context.Update(eventToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AthleteExists(Event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("HostedEvents");
            }
            return View(Event);
        }
        public IActionResult GoCreateNewLocation()
        {
            return RedirectToAction("CreateLocation", "Location");
        }
        
        public async Task<IActionResult> ViewNearLocations()
        {
            var athlete = GetCurrentUser();
            var locations = _context.Locations.Where(l => l.LocationId != 0).ToList();
            List<Location> nearbyLocations = new List<Location>();
            foreach (var place in locations)
            {
                var distanceAway = await _distanceMatrixService.GetDistanceLocation(athlete, place);
                if (distanceAway < athlete.DistanceModifier)
                {
                    nearbyLocations.Add(place);
                }
            }
            return View(nearbyLocations);
        }
        public IActionResult LocationDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = _context.Locations.Find(id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }
        public async Task<IActionResult> ViewNearEvents()
        {
            var athlete = GetCurrentUser();
            var events = _context.Events.Where(e => e.HostAthleteId != athlete.AthleteId).ToList();
            List<Event> nearbyEvents = new List<Event>();
            foreach (var differentEvent in events)
            {
                var distanceAway = await _distanceMatrixService.GetDistanceEvent(athlete, differentEvent);
                if (distanceAway < athlete.DistanceModifier)
                {
                    nearbyEvents.Add(differentEvent);
                }
            }
            return View(nearbyEvents);
        }
        public IActionResult HostedEvents()
        {
            var athlete = GetCurrentUser();
            var hostedEvents = _context.Events.Where(e => e.HostAthleteId == athlete.AthleteId);
            return View(hostedEvents);
        }

        public async Task<IActionResult> EditProfilePicture(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await _context.Athletes.FindAsync(id);
            var athleteVM = new AthleteViewModel();
            if (athlete == null)
            {
                return NotFound();
            }
            ViewBag.Athlete = athlete.ProfilePicture;
            //ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", athlete.IdentityUserId);
            return View(athleteVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfilePicture(int? id, AthleteViewModel athleteVM)
        {
            if (id == null)
            {
                return NotFound();
            }
            var athlete = await _context.Athletes.FindAsync(id);
            ViewBag.Athlete = athlete.ProfilePicture;
            if (athlete == null)
            {
                return NotFound();
            }
            string stringFileName = ProcessUploadedFile(athleteVM);
            athlete.ProfilePicture = stringFileName;
            await _context.SaveChangesAsync();
            //ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", athlete.IdentityUserId);
            return RedirectToAction("Index");
        }
        // GET: Athlete/Edit/5
        public async Task<IActionResult> EditAthlete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await _context.Athletes.FindAsync(id);
            if (athlete == null)
            {
                return NotFound();
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", athlete.IdentityUserId);
            return View(athlete);
        }

        // POST: Athlete/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAthlete(int id, Athlete athlete)
        {
            if (id != athlete.AthleteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var athleteToUpdate = _context.Athletes.Find(id);
                try
                {
                    
                    athleteToUpdate.FirstName = athlete.FirstName;
                    athleteToUpdate.LastName = athlete.LastName;
                    athleteToUpdate.State = athlete.State;
                    athleteToUpdate.ZipCode = athlete.ZipCode;
                    athleteToUpdate.FirstInterest = athlete.FirstInterest;
                    athleteToUpdate.SecondInterest = athlete.SecondInterest;
                    athleteToUpdate.ThirdInterest = athlete.ThirdInterest;
                    athleteToUpdate.AthleticAbility = athlete.AthleticAbility;
                    var updatedGeocodeAthlete = await _geocodingServiceAthlete.GetGeocoding(athlete);
                    athleteToUpdate.AthleteLatitude = updatedGeocodeAthlete.AthleteLatitude;
                    athleteToUpdate.AthleteLongitude = updatedGeocodeAthlete.AthleteLongitude;
                    _context.Update(athleteToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AthleteExists(athlete.AthleteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", athlete.IdentityUserId);
            return View(athlete);
        }

        // GET: Athlete/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var athlete = await _context.Athletes
                .Include(a => a.IdentityUser)
                .FirstOrDefaultAsync(m => m.AthleteId == id);
            if (athlete == null)
            {
                return NotFound();
            }

            return View(athlete);
        }

        // POST: Athlete/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var athlete = await _context.Athletes.FindAsync(id);
            _context.Athletes.Remove(athlete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AthleteExists(int id)
        {
            return _context.Athletes.Any(e => e.AthleteId == id);
        }

        private string ProcessUploadedFile(AthleteViewModel athleteVM)
        {
            string newFileName = null;
            if (athleteVM.ProfilePicture != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                newFileName = Guid.NewGuid().ToString() + "_" + athleteVM.ProfilePicture.FileName;
                string filePath = Path.Combine(uploadsFolder, newFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    athleteVM.ProfilePicture.CopyTo(fileStream);
                }
            }
            return newFileName;
        }

        public Athlete GetCurrentUser()
        {
            var applicationDbContext = _context.Athletes.Include(a => a.IdentityUser);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var athlete = _context.Athletes.Where(o => o.IdentityUserId == userId).FirstOrDefault();
            return athlete;
        }
    }
}

