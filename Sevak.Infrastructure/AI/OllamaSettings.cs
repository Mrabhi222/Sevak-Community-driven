namespace Sevak.Infrastructure.AI;

public class AiSettings
{
    public string BaseUrl { get; set; } = "https://integrate.api.nvidia.com/v1";
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "mistralai/mistral-medium-3.5-128b";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 16384;
}
