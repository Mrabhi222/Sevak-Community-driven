using Microsoft.EntityFrameworkCore;
using Sevak.Application.DTO.Event;
using Sevak.Application.Interfaces;
using Sevak.Domain.Entities;
using Sevak.Domain.Enums;
using Sevak.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly SevakDbContext _context;

    public EventRepository(SevakDbContext context)
    {
        _context = context;
    }

    public Task<Event?> GetByIdAsync(int eventId) =>
        _context.Events.Include(e => e.Organizer).Include(e => e.Volunteers)
        .FirstOrDefaultAsync(e => e.Id == eventId);

    public Task<List<Event>> GetUpcomingAsync(int page, int pageSize) =>
        _context.Events.Where(e => e.EventDate > DateTime.UtcNow && e.Status != EventStatus.Cancelled)
        .OrderBy(e => e.EventDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Include(e => e.Organizer)
        .Include(e => e.Volunteers)
        .ToListAsync();

    public async Task AddAsync(Event @event) => await _context.Events.AddAsync(@event);

    public async Task<bool> UpdateAsync(int eventId, Event @event)
    {
        var existingEvent = await _context.Events.FindAsync(eventId);
        if (existingEvent == null) return false;
        existingEvent.Title = @event.Title;
        existingEvent.Description = @event.Description;
        existingEvent.EventDate = @event.EventDate;
        existingEvent.Location = @event.Location;
        existingEvent.VolunteerCap = @event.VolunteerCap;
        existingEvent.Status = @event.Status;
        return true;
    }

    public async Task<bool> DeleteAsync(int eventId)
    {
        var @event = await _context.Events.FindAsync(eventId);
        if (@event == null) return false;
        _context.Events.Remove(@event);
        return true;
    }

    public async Task<bool> SignUpVolunteerAsync(int eventId, int volunteerId)
    {
        var @event = await _context.Events.Include(e => e.Volunteers)
             .FirstOrDefaultAsync(e => e.Id == eventId);
        if (@event == null || @event.Volunteers.Count >= @event.VolunteerCap) return false;
        _context.EventVolunteers.Add(new EventVolunteer { EventId = eventId, VolunteerId = volunteerId });
        return true;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();



}