using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace ConsoleApp5
{
    #region Entities

    public class Organizer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? CompanyName { get; set; }

        public bool IsVerified { get; set; }

        public OrganizerProfile Profile { get; set; }

        public ICollection<Event> Events { get; set; }
    }

    public class OrganizerProfile
    {
        public int Id { get; set; }

        public string Biography { get; set; }

        public string Website { get; set; }

        public string Logo { get; set; }

        public int OrganizerId { get; set; }
        public Organizer Organizer { get; set; }
    }

    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int MaxAttendees { get; set; }

        public int OrganizerId { get; set; }

        public Organizer Organizer { get; set; }

        public ICollection<Session> Sessions { get; set; }

        public ICollection<Registration> Registrations { get; set; }
    }

    public class Session
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime StartTime { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }

    public class Attendee
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public Badge Badge { get; set; }

        public ICollection<Registration> Registrations { get; set; }
    }

    public class Badge
    {
        public int Id { get; set; }

        public DateTime IssueDate { get; set; }

        public string Tier { get; set; }

        public int AttendeeId { get; set; }

        public Attendee Attendee { get; set; }
    }

    public class Registration
    {
        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string? Note { get; set; }
    }

    #endregion

    public class EventHubContext : DbContext
    {
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<OrganizerProfile> OrganizerProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Badge> Badges { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=.;Database=EventHubDB;Trusted_Connection=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organizer>()
                .HasOne(o => o.Profile)
                .WithOne(p => p.Organizer)
                .HasForeignKey<OrganizerProfile>(p => p.OrganizerId);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganizerId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Sessions)
                .HasForeignKey(s => s.EventId);

            modelBuilder.Entity<Attendee>()
                .HasOne(a => a.Badge)
                .WithOne(b => b.Attendee)
                .HasForeignKey<Badge>(b => b.AttendeeId);

            modelBuilder.Entity<Registration>()
                .HasKey(r => new { r.AttendeeId, r.EventId });

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Attendee)
                .WithMany(a => a.Registrations)
                .HasForeignKey(r => r.AttendeeId);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(r => r.EventId);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new EventHubContext();
            db.Database.EnsureCreated();

            Console.WriteLine("Database Created Successfully ✅");
        }
    }
}