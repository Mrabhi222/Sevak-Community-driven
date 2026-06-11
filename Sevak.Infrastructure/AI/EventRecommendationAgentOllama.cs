namespace Sevak.Infrastructure.AI;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sevak.Infrastructure.Data;
using System.Text.Json;

public class EventRecommendationAgent
{
    private readonly SevakDbContext _context;
    private readonly OllamaApiClient _ollamaClient;
    private readonly ILogger<EventRecommendationAgent> _logger;

    public EventRecommendationAgent(
        SevakDbContext context,
        OllamaApiClient ollamaClient,
        ILogger<EventRecommendationAgent> logger)
    {
        _context = context;
        _ollamaClient = ollamaClient;
        _logger = logger;
    }

    public async Task<List<RecommendedEventDto>> GetRecommendationsAsync(
        int volunteerId,
        int limit = 5,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting recommendations for volunteer {VolunteerId}", volunteerId);

        try
        {
            // Get volunteer
            var volunteer = await _context.Users
                .Include(u => u.VolunteerRegistrations)
                .FirstOrDefaultAsync(u => u.Id == volunteerId, cancellationToken);

            if (volunteer == null)
                throw new KeyNotFoundException("Volunteer not found");

            // Get upcoming events (not already registered)
            var events = await _context.Events
                .Where(e => e.EventDate > DateTime.UtcNow &&
                            !e.Volunteers.Any(v => v.VolunteerId == volunteerId))
                .Include(e => e.Volunteers)
                .ToListAsync(cancellationToken);

            if (events.Count == 0)
            {
                _logger.LogInformation("No upcoming events available");
                return new List<RecommendedEventDto>();
            }

            // Build prompt
            var prompt = BuildPrompt(volunteer, events, limit);

            // Call Ollama
            var response = await _ollamaClient.GenerateAsync(prompt, cancellationToken);

            // Parse JSON response
            var recommendations = ParseRecommendations(response, events);

            _logger.LogInformation("Generated {Count} recommendations", recommendations.Count);
            return recommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations");
            throw;
        }
    }

    private string BuildPrompt(Domain.Entities.User volunteer, List<Domain.Entities.Event> events, int limit)
    {
        var volunteerInfo = $"""
            Volunteer Profile:
            - Name: {volunteer.Name}
            - Location: {volunteer.Location ?? "Not specified"}
            - Skills: {string.Join(", ", volunteer.Skills ?? new List<string> { "general volunteering" })}
            - Past Events: {volunteer.VolunteerRegistrations.Count}
            - Total Hours: {volunteer.VolunteerRegistrations.Sum(v => v.HoursLogged)}
            """;

        var eventsList = string.Join("\n\n", events.Take(10).Select((e, i) => $"""
            Event {i + 1} (ID: {e.Id}):
            - Title: {e.Title}
            - Location: {e.Location}
            - Date: {e.EventDate:yyyy-MM-dd HH:mm}
            - Spots Available: {e.VolunteerCap - e.Volunteers.Count}/{e.VolunteerCap}
            - Description: {e.Description}
            """));

        return $$"""
            Recommend the TOP {{limit}} most suitable events for this volunteer.
            
            {{volunteerInfo}}
            
            AVAILABLE EVENTS:
            {{eventsList}}
            
            Return ONLY a valid JSON array using the exact IDs provided above. No markdown, no extra text.
            Example format: [{"eventId": <use exact ID from above>, "title": "...", "location": "...", "score": 95, "reason": "..."}]
            Include only fields: eventId, title, location, score (0-100), reason.
            """;
    }

    private List<RecommendedEventDto> ParseRecommendations(string jsonResponse, List<Domain.Entities.Event> events)
    {
        try
        {
            var cleanJson = jsonResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var recommendations = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(cleanJson)
                ?? new List<Dictionary<string, JsonElement>>();

            var result = new List<RecommendedEventDto>();

        var validIds = events.Select(e => e.Id).ToHashSet();

        foreach (var rec in recommendations)
            try
            {
                var dto = new RecommendedEventDto
                {
                    EventId = rec.ContainsKey("eventId") ? rec["eventId"].GetInt32() : 0,
                    Title = rec.ContainsKey("title") ? rec["title"].GetString() ?? "" : "",
                    Location = rec.ContainsKey("location") ? rec["location"].GetString() ?? "" : "",
                    RecommendationScore = rec.ContainsKey("score") ? rec["score"].GetDouble() : 0,
                    RecommendationReason = rec.ContainsKey("reason") ? rec["reason"].GetString() ?? "" : "",
                    VolunteerCap = 0,
                    VolunteersCurrent = 0
                };

                if (validIds.Contains(dto.EventId))
                    result.Add(dto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse individual recommendation");
            }

            return result;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Ollama response as JSON: {Response}", jsonResponse);
            return new List<RecommendedEventDto>();
        }
    }
}

public class RecommendedEventDto
{
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public double RecommendationScore { get; set; }
    public string RecommendationReason { get; set; }
    public int VolunteerCap { get; set; }
    public int VolunteersCurrent { get; set; }
}