using Sevak.Application.DTO.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Application.Interfaces;

public interface IEventService
{
    Task<EventDetailDto> CreateEventAsync(CreateEventDto dto, int organizerId);
    Task<List<EventDetailDto>> GetUpcomingEventsAsync(int page = 1);
    Task<EventDetailDto> GetEventByIdAsync(int eventId);
    Task<bool> UpdateEventAsync(int eventId, CreateEventDto dto);
    Task<bool> DeleteEventAsync(int eventId);
    Task<bool> SignUpVolunteerAsync(int eventId, int volunteerId);
}
