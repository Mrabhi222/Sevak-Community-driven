using Sevak.Application.DTO.Common;

namespace Sevak.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new { success = false, message = "An error occurred while processing your request." };
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      
        context.Response.ContentType = "application/json";
        var response = new ApiResponseDto<object>
        {
            Success = false,
            Message = "An error occurred while processing your request."
        };
        switch (exception)
        {
            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "Resource not found.";
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Unauthorized access.";
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}
