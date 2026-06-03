using Sevak.Domain.Entities;

namespace Sevak.Application.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int eventId);
    Task<List<Event>> GetUpcomingAsync(int page, int pageSize);
    Task AddAsync(Event @event);
    Task<bool> UpdateAsync(int eventId, Event @event);
    Task<bool> DeleteAsync(int eventId);
    Task<bool> SignUpVolunteerAsync(int eventId, int volunteerId);
    Task SaveChangesAsync();
}
