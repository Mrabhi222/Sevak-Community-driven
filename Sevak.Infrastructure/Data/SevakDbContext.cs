using System;
using System.Collections.Generic;
using Sevak.Domain.Entities;
using Sevak.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Sevak.Infrastructure.Data;

public class SevakDbContext : DbContext
{
    public SevakDbContext(DbContextOptions<SevakDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventVolunteer> EventVolunteers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Review> Reviews { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert enums to strings in PostgreSQL
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasDefaultValue(UserRole.Volunteer);

        modelBuilder.Entity<Event>()
            .Property(e => e.Status)
            .HasConversion<string>()
            .HasDefaultValue(EventStatus.Draft);

        modelBuilder.Entity<EventVolunteer>()
            .Property(ev => ev.Status)
            .HasConversion<string>()
            .HasDefaultValue(VolunteerStatus.Registered);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Store Skills as JSON array in PostgreSQL
        modelBuilder.Entity<User>()
            .Property(u => u.Skills)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? "[]" : System.Text.Json.JsonSerializer.Serialize(v),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>()
            );

        // Event configuration
        modelBuilder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Event>()
            .Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<Event>()
            .Property(e => e.Description)
            .HasMaxLength(2000);

        modelBuilder.Entity<Event>()
            .Property(e => e.Location)
            .HasMaxLength(300)
            .IsRequired();

        // EventVolunteer configuration
        modelBuilder.Entity<EventVolunteer>()
            .HasOne(ev => ev.Event)
            .WithMany(e => e.Volunteers)
            .HasForeignKey(ev => ev.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventVolunteer>()
            .HasOne(ev => ev.Volunteer)
            .WithMany(u => u.VolunteerRegistrations)
            .HasForeignKey(ev => ev.VolunteerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: One user can't sign up for same event twice
        modelBuilder.Entity<EventVolunteer>()
            .HasIndex(ev => new { ev.EventId, ev.VolunteerId })
            .IsUnique();

        // Review configuration
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Reviews)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .Property(r => r.Rating)
            .HasDefaultValue(5);

        // Notification configuration
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasMaxLength(50);

        modelBuilder.Entity<Notification>()
            .Property(n => n.Message)
            .HasMaxLength(500);

        // Create indexes for better query performance
        modelBuilder.Entity<Event>()
            .HasIndex(e => e.EventDate);

        modelBuilder.Entity<Event>()
            .HasIndex(e => e.Status);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email);
    }
}
