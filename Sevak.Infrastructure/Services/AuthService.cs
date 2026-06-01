using BCrypt.Net;
using BcryptNet = BCrypt.Net.BCrypt;
using Sevak.Application.DTO.Auth;
using Sevak.Application.Interfaces;
using Sevak.Domain.Entities;

namespace Sevak.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);

        if (user == null || !BcryptNet.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("User account is inactive");

        return ToResponse(user);
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.FindByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BcryptNet.HashPassword(request.Password),
            Role = request.Role,
            Location = request.Location,
            IsActive = true
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return ToResponse(user);
    }

    private LoginResponseDto ToResponse(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.ToString(),
        AccessToken = _tokenService.GenerateAccessToken(user),
        RefreshToken = _tokenService.GenerateRefreshToken()
    };
}