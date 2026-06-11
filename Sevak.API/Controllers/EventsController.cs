using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sevak.Application.DTO.Common;
using Sevak.Application.DTO.Event;
using Sevak.Application.Interfaces;
using Sevak.Infrastructure.AI;
using System.Collections.Generic;

namespace Sevak.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly EventRecommendationAgent _recommendationAgent;

        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger, EventRecommendationAgent recommendationAgent)
        {
            _eventService = eventService;
            _logger = logger;
            _recommendationAgent = recommendationAgent;

        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<EventDetailDto>>>> GetUpcomingEvents([FromQuery] int page = 1)
        {
            try
            {
                var events = await _eventService.GetUpcomingEventsAsync(page);
                return Ok(new ApiResponseDto<List<EventDetailDto>>
                {
                    Success = true,
                    Data = events,
                    Message = $"Retrieved {events.Count} events"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching events");
                return StatusCode(500, new ApiResponseDto<List<EventDetailDto>>
                {
                    Success = false,
                    Message = "An error occurred while fetching events"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<EventDetailDto>>> GetEventById(int id)
        {
            try
            {
                var @event = await _eventService.GetEventByIdAsync(id);
                return Ok(new ApiResponseDto<EventDetailDto>
                {
                    Success = true,
                    Data = @event
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching event {id}");
                return NotFound(new ApiResponseDto<EventDetailDto>
                {
                    Success = false,
                    Message = "Event not found"
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<ActionResult<ApiResponseDto<EventDetailDto>>> CreateEvent([FromBody] CreateEventDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponseDto<EventDetailDto> { Success = false, Message = "Invalid input" });

                var userIdClaim = User.FindFirst("sub");
                if (!int.TryParse(userIdClaim?.Value, out var userId))
                    return Unauthorized(new ApiResponseDto<EventDetailDto> { Success = false, Message = "Invalid user" });

                var result = await _eventService.CreateEventAsync(dto, userId);
                return CreatedAtAction(nameof(GetEventById), new { id = result.Id },
                    new ApiResponseDto<EventDetailDto>
                    {
                        Success = true,
                        Data = result,
                        Message = "Event created successfully"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return StatusCode(500, new ApiResponseDto<EventDetailDto>
                {
                    Success = false,
                    Message = "An error occurred while creating the event"
                });
            }
        }

        [HttpPost("{id}/signup")]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<ApiResponseDto<bool>>> SignUpVolunteer(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst("sub");
                if (!int.TryParse(userIdClaim?.Value, out var userId))
                    return Unauthorized(new ApiResponseDto<bool> { Success = false, Message = "Invalid user" });

                var result = await _eventService.SignUpVolunteerAsync(id, userId);
                return Ok(new ApiResponseDto<bool>
                {
                    Success = result,
                    Data = result,
                    Message = result ? "Successfully signed up for event" : "Failed to sign up"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error signing up for event {id}");
                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("recommendations")]
        [Authorize(Roles = "Volunteer")]
        public async Task<ActionResult<ApiResponseDto<List<RecommendedEventDto>>>> GetRecommendations(
    [FromQuery] int limit = 5)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");

                var recommendations = await _recommendationAgent.GetRecommendationsAsync(userId, limit);

                return Ok(new ApiResponseDto<List<RecommendedEventDto>>
                {
                    Success = true,
                    Data = recommendations,
                    Message = $"Found {recommendations.Count} recommended events"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Ollama service unavailable");
                return StatusCode(503, new ApiResponseDto<List<RecommendedEventDto>>
                {
                    Success = false,
                    Message = "AI service unavailable. Make sure Ollama is running.",
                    Errors = new List<string> { ex.Message }
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto<List<RecommendedEventDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommendations");
                return StatusCode(500, new ApiResponseDto<List<RecommendedEventDto>>
                {
                    Success = false,
                    Message = "Error generating recommendations"
                });
            }
        }
    }
}
