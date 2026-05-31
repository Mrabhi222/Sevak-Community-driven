using System;
using System.Collections.Generic;
using System.Text;
using Sevak.Domain.Enums;

namespace Sevak.Domain.Entities;

public class EventVolunteer
{
    public int Id { get; set; }

    // Foreign keys
    public int EventId { get; set; }
    public Event Event { get; set; }

    public int VolunteerId { get; set; }
    public User Volunteer { get; set; }

    // Data
    public DateTime SignUpDate { get; set; } = DateTime.UtcNow;
    public decimal HoursLogged { get; set; } = 0;
    public VolunteerStatus Status { get; set; } = VolunteerStatus.Registered;
}
