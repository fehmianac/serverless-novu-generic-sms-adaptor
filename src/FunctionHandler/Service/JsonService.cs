using System.Text.Json;
using FunctionHandler.Contract;

namespace FunctionHandler.Service;

public class JsonService : IJsonService
{
    public T? DeserializeAsync<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}