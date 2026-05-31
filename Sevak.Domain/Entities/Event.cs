using System;
using System.Collections.Generic;
using System.Text;
using Sevak.Domain.Enums; // ✅ add this


namespace Sevak.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public int VolunteerCap { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;

    // Foreign key
    public int OrganizerId { get; set; }
    public User Organizer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public ICollection<EventVolunteer> Volunteers { get; set; } = new List<EventVolunteer>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
