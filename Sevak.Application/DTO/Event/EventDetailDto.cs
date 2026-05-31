using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Application.DTO.Event;

public class EventDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public int VolunteerCap { get; set; }
    public int VolunteersSignedUp { get; set; }
    public string Status { get; set; }
    public OrganizerDto Organizer { get; set; }
}

public class OrganizerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
