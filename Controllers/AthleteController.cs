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
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Athletes.Include(a => a.IdentityUser);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var athlete = _context.Athletes.Where(o => o.IdentityUserId == userId).FirstOrDefault();
            if (athlete == null)
            {
                return RedirectToAction("CreateVM");
            }
            var view = _context.Athletes.Where(a => a.AthleteId == athlete.AthleteId).ToList();
            return View(view);
        }

        // GET: Athlete/Details/5
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
            athleteWithLatandLong.DistanceModifier = 20;

            if(athleteWithLatandLong.ProfilePicture == null)
            {
                athleteWithLatandLong.ProfilePicture = "anon.png";
            }
            _context.Add(athleteWithLatandLong);
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

        // GET: Athlete/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id, [Bind("AthleteId,IdentityUserId")] Athlete athlete)
        {
            if (id != athlete.AthleteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(athlete);
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
                return RedirectToAction(nameof(Index));
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
    }
}

