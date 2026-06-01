using System.ComponentModel.DataAnnotations;
using Sevak.Domain.Enums;

namespace Sevak.Application.DTO.Auth;

public class RegisterRequestDto
{
    [Required] public string Name { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public UserRole Role { get; set; }
    public string? Location { get; set; }
}
