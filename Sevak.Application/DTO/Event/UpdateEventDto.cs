namespace Sevak.Application.DTO.Event;

public class UpdateEventDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public int VolunteerCap { get; set; }
}
