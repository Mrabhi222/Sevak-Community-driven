using Microsoft.EntityFrameworkCore;
using Sevak.Application.Interfaces;
using Sevak.Domain.Entities;
using Sevak.Infrastructure.Data;

namespace Sevak.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SevakDbContext _context;

    public UserRepository(SevakDbContext context) => _context = context;

    public Task<User?> FindByEmailAsync(string email) =>
        _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user) => await _context.Users.AddAsync(user);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
