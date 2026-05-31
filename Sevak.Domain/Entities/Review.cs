using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Domain.Entities;

public class Review
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }

    public int ReviewerId { get; set; }
    public User Reviewer { get; set; }

    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
