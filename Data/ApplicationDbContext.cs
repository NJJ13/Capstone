using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using FreshAir.Models;

namespace FreshAir.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Athlete", NormalizedName = "ATHLETE" });

            builder.Entity<AthleteEvent>().HasKey(ae => new { ae.AthleteId, ae.EventId });
            builder.Entity<AthleteEvent>().HasOne(ae => ae.Athlete).WithMany(ae => ae.Events).HasForeignKey(ae => ae.AthleteId);
            builder.Entity<AthleteEvent>().HasOne(ae => ae.Event).WithMany(ae => ae.Attendees).HasForeignKey(ae => ae.EventId);

        }
        public DbSet<Athlete> Athletes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
