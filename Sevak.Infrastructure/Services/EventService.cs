using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sevak.Application.DTO.Event;
using Sevak.Application.Interfaces;
using Sevak.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly SevakDbContext _context;
    private readonly ILogger<EventService> _logger;

    public EventService(SevakDbContext context, ILogger<EventService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EventDetailDto> CreateEventAsync(CreateEventDto dto, int organizerId)
    {
        var Organizer = await _context.Organizers.FindAsync(organizerId);
        if (Organizer == null)
        {
            _logger.LogWarning("Attempt to create event with non-existent organizer ID {OrganizerId}", organizerId);
            throw new KeyNotFoundException("Organizer not found");
        }


    }
}
