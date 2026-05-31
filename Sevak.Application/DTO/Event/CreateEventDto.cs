using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Application.DTO.Event;

public class CreateEventDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public int VolunteerCap { get; set; }
}
