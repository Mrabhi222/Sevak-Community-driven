using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Application.DTO.Auth;

public class RegisterRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } // "Organizer" or "Volunteer"
    public string? Location { get; set; }
}
