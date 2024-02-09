using System.Text.Json.Serialization;

namespace FunctionHandler.Model;

public class RequestModel
{
    [JsonPropertyName("sender")] public string Sender { get; set; } = default!;

    [JsonPropertyName("to")] public string To { get; set; } = default!;

    [JsonPropertyName("content")] public string Content { get; set; } = default!;
    [JsonPropertyName("from")] public string From { get; set; } = default!;

    [JsonPropertyName("id")] public string? Id { get; set; } = default!;

    [JsonPropertyName("customData")] public Dictionary<string, object>? CustomData { get; set; } = new();
}