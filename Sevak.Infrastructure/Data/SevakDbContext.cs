using System.Reflection;
using Sevak.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Sevak.Infrastructure.Data;

public class SevakDbContext : DbContext
{
    public SevakDbContext(DbContextOptions<SevakDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventVolunteer> EventVolunteers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
