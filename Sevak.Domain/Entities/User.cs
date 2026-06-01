using Sevak.Domain.Enums;

namespace Sevak.Domain.Entities;

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public List<string>? Skills { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<Event> OrganizedEvents { get; set; } = new List<Event>();
        public ICollection<EventVolunteer> VolunteerRegistrations { get; set; } = new List<EventVolunteer>();
    }

