using System.Text.Json.Serialization;

namespace FunctionHandler.Model;

public class ResponseModel
{
    [JsonPropertyName("id")] public string Id { get; set; } = default!;

    [JsonPropertyName("date")] public string Date { get; set; } = default!;
}