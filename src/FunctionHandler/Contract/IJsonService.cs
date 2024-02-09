namespace FunctionHandler.Contract;

public interface IJsonService
{
    T? DeserializeAsync<T>(string json);
}