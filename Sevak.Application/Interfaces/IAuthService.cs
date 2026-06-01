using Sevak.Application.DTO.Auth;

namespace Sevak.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
}

public interface ITokenSessionService
{
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(int userId);
}
