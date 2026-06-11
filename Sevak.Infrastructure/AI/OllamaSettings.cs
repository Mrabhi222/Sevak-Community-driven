using System;
using System.Collections.Generic;
using System.Text;

namespace Sevak.Infrastructure.AI;

public class OllamaSettings
{
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";
    public string OllamaModel { get; set; } = "mistral";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 500;
}
