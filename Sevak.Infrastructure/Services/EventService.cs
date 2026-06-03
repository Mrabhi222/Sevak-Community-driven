using Sevak.Application.DTO.Event;
using Sevak.Application.Interfaces;
using Sevak.Domain.Entities;
using Sevak.Domain.Enums;

namespace Sevak.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository) => _eventRepository = eventRepository;

    public async Task<EventDetailDto> CreateEventAsync(CreateEventDto dto, int organizerId)
    {
        var @event = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Location = dto.Location,
            VolunteerCap = dto.VolunteerCap,
            OrganizerId = organizerId,
            Status = EventStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _eventRepository.AddAsync(@event);
        await _eventRepository.SaveChangesAsync();

        return await GetEventByIdAsync(@event.Id);
    }

    public async Task<List<EventDetailDto>> GetUpcomingEventsAsync(int page = 1)
    {
        var events = await _eventRepository.GetUpcomingAsync(page, 10);
        return events.Select(ToDto).ToList();
    }

    public async Task<EventDetailDto> GetEventByIdAsync(int eventId)
    {
        var @event = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new KeyNotFoundException("Event not found");
        return ToDto(@event);
    }

    public async Task<bool> UpdateEventAsync(int eventId, UpdateEventDto dto)
    {
        var updated = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            Location = dto.Location,
            VolunteerCap = dto.VolunteerCap
        };

        var result = await _eventRepository.UpdateAsync(eventId, updated);
        if (result) await _eventRepository.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        var result = await _eventRepository.DeleteAsync(eventId);
        if (result) await _eventRepository.SaveChangesAsync();
        return result;
    }

    public async Task<bool> SignUpVolunteerAsync(int eventId, int volunteerId)
    {
        var result = await _eventRepository.SignUpVolunteerAsync(eventId, volunteerId);
        if (result) await _eventRepository.SaveChangesAsync();
        return result;
    }

    private static EventDetailDto ToDto(Event e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        EventDate = e.EventDate,
        Location = e.Location,
        VolunteerCap = e.VolunteerCap,
        VolunteersSignedUp = e.Volunteers?.Count ?? 0,
        Status = e.Status.ToString(),
        Organizer = e.Organizer == null ? null : new OrganizerDto
        {
            Id = e.Organizer.Id,
            Name = e.Organizer.Name,
            Email = e.Organizer.Email
        }
    };
}
