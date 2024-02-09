using System.Text.Json.Serialization;

namespace FunctionHandler.Model;

public class ApiKeyResponse
{
    [JsonPropertyName("x-api-key")]
    public string ApiKey { get; set; } = default!;
}