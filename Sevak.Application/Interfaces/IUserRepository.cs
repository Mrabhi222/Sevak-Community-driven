using Sevak.Domain.Entities;

namespace Sevak.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
