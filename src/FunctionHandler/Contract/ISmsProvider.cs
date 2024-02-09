using FunctionHandler.Enum;

namespace FunctionHandler.Contract;

public interface ISmsProvider
{
    Providers Provider { get; }
    IProviderSettings ProviderSettings { get; }
    Task<bool> SendAsync(string to, string from, string message, CancellationToken cancellationToken);
}