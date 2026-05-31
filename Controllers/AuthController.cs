namespace Sevak.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Sevak.Application.DTO.Auth;
using Sevak.Application.DTO.Common;
using Sevak.Application.DTO.Auth;
using Sevak.Application.DTO.Common;
using Sevak.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponseDto<LoginResponseDto> { Success = false, Message = "Invalid input" });

            var result = await _authService.LoginAsync(request);
            return Ok(new ApiResponseDto<LoginResponseDto>
            {
                Success = true,
                Data = result,
                Message = "Login successful"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning($"Login failed: {ex.Message}");
            return Unauthorized(new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "An error occurred during login"
            });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponseDto<LoginResponseDto> { Success = false, Message = "Invalid input" });

            var result = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Login), new ApiResponseDto<LoginResponseDto>
            {
                Success = true,
                Data = result,
                Message = "Registration successful"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Registration failed: {ex.Message}");
            return BadRequest(new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"Registration validation failed: {ex.Message}");
            return BadRequest(new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error");
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "An error occurred during registration"
            });
        }
    }
}