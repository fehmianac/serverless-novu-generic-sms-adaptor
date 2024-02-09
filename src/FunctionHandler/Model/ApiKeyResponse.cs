using System.Text.Json.Serialization;

namespace FunctionHandler.Model;

public class ApiKeyResponse
{
    [JsonPropertyName("Authorization")]
    public string Authorization { get; set; } = default!;
}