namespace FunctionHandler.Contract;

public interface ISmsSender
{
    bool ValidateApiKey(string apiKey, CancellationToken cancellationToken = default);
    Task<bool> SendAsync(string to, string from, string message, CancellationToken cancellationToken = default);
}