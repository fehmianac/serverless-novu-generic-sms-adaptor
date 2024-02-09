using FunctionHandler.Options;

namespace FunctionHandler.Contract;

public interface IParameterService
{
    Task<Settings> GetSettingsAsync(CancellationToken cancellationToken = default);
}