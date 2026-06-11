namespace Sevak.Infrastructure.AI;

using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

public class OllamaApiClient
{
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;
    private readonly ILogger<OllamaApiClient> _logger;

    public OllamaApiClient(
        HttpClient httpClient,
        OllamaSettings settings,
        ILogger<OllamaApiClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling Ollama API");

        var requestBody = new OllamaRequest
        {
            Model = _settings.OllamaModel,
            Prompt = prompt,
            Temperature = _settings.Temperature,
            NumPredict = _settings.MaxTokens,
            Stream = false
        };

        try
        {
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"{_settings.OllamaBaseUrl}/api/generate",
                content,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonResponse = JsonDocument.Parse(responseContent);

            var result = jsonResponse.RootElement.GetProperty("response").GetString() ?? "";

            _logger.LogInformation("Ollama response received, length: {Length}", result.Length);
            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to connect to Ollama at {Url}", _settings.OllamaBaseUrl);
            throw new InvalidOperationException(
                $"Cannot connect to Ollama. Make sure it's running: 'ollama serve'",
                ex);
        }
    }
}

public class OllamaRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("num_predict")]
    public int NumPredict { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
}