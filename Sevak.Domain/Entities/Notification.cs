using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

    public string Message { get; set; }
    public string Type { get; set; } // "EventUpdate", "SignUp", "HourLogged"
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
