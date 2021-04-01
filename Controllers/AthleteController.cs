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
            var currentUser = GetCurrentUser();
            if (id == currentUser.AthleteId)
            {
                return RedirectToAction("MyDetails");
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
                newEvent.LocationsName = location.LocationName;
                var athlete = GetCurrentUser();
                newEvent.HostAthlete = athlete;
                newEvent.HostAthleteId = athlete.AthleteId;
                newEvent.AttendanceCount = 1;

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
        public IActionResult AttendedEventDetails(int? id)
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
        public IActionResult MyEventDetails(int? id)
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
        public IActionResult MyExpiredEventDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expiredEvent = _context.Events.Find(id);
            if (expiredEvent == null)
            {
                return NotFound();
            }
            var hostAthlete = _context.Athletes.Find(expiredEvent.HostAthleteId);
            var location = _context.Locations.Find(expiredEvent.LocationId);
            ViewBag.HostPicture = hostAthlete.ProfilePicture;
            ViewBag.HostAthleteName = (hostAthlete.FirstName + " " + hostAthlete.LastName);
            ViewBag.LocationPicture = location.Picture;
            ViewBag.LocationDescription = location.Description;
            return View(expiredEvent);
        }
        public async Task<IActionResult> AttendEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var attendingAthlete = GetCurrentUser();
            var eventToAttend = _context.Events.Find(id);
            eventToAttend.AttendanceCount += 1;
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
            _context.Events.Update(eventToAttend);
            _context.AthleteEvents.Add(athleteEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("EventsAttending");
        }
        public async Task<IActionResult> LeaveEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var athlete = GetCurrentUser();
            var eventToLeave = _context.Events.Find(id);
            eventToLeave.AttendanceCount -= 1;
            if (eventToLeave == null)
            {
                return NotFound();
            }
            var removeAthleteEvent = _context.AthleteEvents.Find(athlete.AthleteId, eventToLeave.EventId);
            _context.Events.Update(eventToLeave);
            _context.AthleteEvents.Remove(removeAthleteEvent);
            await _context.SaveChangesAsync();

            return RedirectToAction("EventsAttending");
        }

        public IActionResult EventsAttending()
        {
            var athlete = GetCurrentUser();
            var athletesEvents = _context.AthleteEvents.Where(ae => ae.AthleteId == athlete.AthleteId).ToList();
            List<Event> currentAttendingEvents = new List<Event>();
            foreach (var item in athletesEvents)
            {
                var attendedEvent = _context.Events.Find(item.EventId);
                currentAttendingEvents.Add(attendedEvent);
            }
            var events = RemoveExpiredEvents(currentAttendingEvents);
            return View(events);
        }
        
        public IActionResult AthletesAttending(int? id)
        {
            var athletesAttendingId = _context.AthleteEvents.Where(ae => ae.EventId == id).ToList();
            List<Athlete> attendingAthletes = new List<Athlete>();
            foreach (var item in athletesAttendingId)
            {
                attendingAthletes.Add(_context.Athletes.Find(item.AthleteId));
            }
            return View(attendingAthletes);
        }
        public async Task<IActionResult> SuggestedEvents()
        {
            var athlete = GetCurrentUser();
            var events = _context.Events.Where(e => e.HostAthleteId != athlete.AthleteId).ToList();
            var eventsAttended = _context.AthleteEvents.Where(ae => ae.AthleteId == athlete.AthleteId).ToList();
            foreach (var item in eventsAttended)
            {
                var eventToRemove = _context.Events.Find(item.EventId);
                events.Remove(eventToRemove);

            }
            events = RemoveExpiredEvents(events);
            List<Event> suggestedEvents = new List<Event>();
            foreach (var differentEvent in events)
            {
                var distanceAway = await _distanceMatrixService.GetDistanceEvent(athlete, differentEvent);
                if (distanceAway < athlete.DistanceModifier)
                {
                    if (differentEvent.Activity == athlete.FirstInterest || differentEvent.Activity == athlete.SecondInterest || differentEvent.Activity == athlete.ThirdInterest)
                    {
                        suggestedEvents.Add(differentEvent);
                    }
                }
            }
            return View(suggestedEvents);
            
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
        public IActionResult LocationsEvents(int? id)
        {
            var athlete = GetCurrentUser();
            var eventsAtLocation = _context.Events.Where(e => e.LocationId == id).ToList();
            var eventsAttended = _context.AthleteEvents.Where(ae => ae.AthleteId == athlete.AthleteId).ToList();
            foreach (var item in eventsAttended)
            {
                var eventToRemove = _context.Events.Find(item.EventId);
                eventsAtLocation.Remove(eventToRemove);
            }
            var locationEvents = RemoveExpiredEvents(eventsAtLocation);

            return View(locationEvents);
        }
        public async Task<IActionResult> ViewNearEvents()
        {
            var athlete = GetCurrentUser();
            var events = _context.Events.Where(e => e.HostAthleteId != athlete.AthleteId).ToList();
            var eventsAttended = _context.AthleteEvents.Where(ae => ae.AthleteId == athlete.AthleteId).ToList();
            foreach (var item in eventsAttended)
            {
                var eventToRemove = _context.Events.Find(item.EventId);
                events.Remove(eventToRemove);
            }
            List<Event> nearbyEvents = new List<Event>();
            foreach (var differentEvent in events)
            {
                var distanceAway = await _distanceMatrixService.GetDistanceEvent(athlete, differentEvent);
                if (distanceAway < athlete.DistanceModifier)
                {
                    nearbyEvents.Add(differentEvent);
                }
            }
            var nearbyEventsWithoutExpiredEvents = RemoveExpiredEvents(nearbyEvents);
            return View(nearbyEventsWithoutExpiredEvents);
        }
        public IActionResult HostedEvents()
        {
            var athlete = GetCurrentUser();
            var hostedEvents = _context.Events.Where(e => e.HostAthleteId == athlete.AthleteId).ToList();
            var activeHostedEvents = RemoveExpiredEvents(hostedEvents);
            return View(activeHostedEvents);
        }
        public IActionResult PastHostedEvents()
        {
            var athlete = GetCurrentUser();
            var hostedEvents = _context.Events.Where(e => e.HostAthleteId == athlete.AthleteId).ToList();
            var pastHostedEvents = RemoveActiveEvents(hostedEvents);
            return View(pastHostedEvents);
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
        public async Task<IActionResult> RemoveEvent(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventToRemove = await _context.Events.FindAsync(id);
                
            if (eventToRemove == null)
            {
                return NotFound();
            }

            return View(eventToRemove);
        }

        // POST: Athlete/Delete/5
        [HttpPost, ActionName("RemoveEvent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var eventToRemove = await _context.Events.FindAsync(id);
            _context.Events.Remove(eventToRemove);
            var updateAthleteEvents = _context.AthleteEvents.Where(ae => ae.EventId == eventToRemove.EventId).ToList();
            foreach (var item in updateAthleteEvents)
            {
                _context.AthleteEvents.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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

        public List<Event> RemoveExpiredEvents(List<Event> events)
        {
            var eventsWithoutExpired = events.Where(e => e.ScheduledTIme.Value.CompareTo(DateTime.Now) >= 0).ToList();
            
            return eventsWithoutExpired;
        }
        public List<Event> RemoveActiveEvents(List<Event> events)
        {
            var expiredEvents = events.Where(e => e.ScheduledTIme.Value.CompareTo(DateTime.Now) < 0).ToList();

            return expiredEvents;
        }
    }
}

