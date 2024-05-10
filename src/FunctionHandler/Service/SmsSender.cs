using FunctionHandler.Contract;
using FunctionHandler.Options;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FunctionHandler.Service;

public class SmsSender : ISmsSender
{
    private readonly IList<ISmsProvider> _providers = new List<ISmsProvider>();
    private readonly Settings _settings;

    public SmsSender(Settings settings)
    {
        _settings = settings;
        if (settings.Twilio.IsEnabled)
            _providers.Add(new TwilioSmsProvider(settings));

        if (settings.NetGsm.IsEnabled)
            _providers.Add(new NetGsmSmsProvider(settings));

        if (!_providers.Any())
            throw new InvalidOperationException("No SMS provider is enabled");
    }

    public SmsSender(Settings settings, IList<ISmsProvider> providers)
    {
        _settings = settings;
        _providers = providers;
    }

    public bool ValidateApiKey(string apiKey, CancellationToken cancellationToken = default)
    {
        return _settings.ApiKey == apiKey;
    }

    public async Task<bool> SendAsync(string to, string from, string message,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine(JsonSerializer.Serialize(_settings));
        var trimmedTo = to.TrimStart('+');
        var providerByCountyCode =
            _providers.FirstOrDefault(q => q.ProviderSettings.AllowedCountryCodes.Any(x => trimmedTo.StartsWith(x)));

        if (providerByCountyCode == null)
            providerByCountyCode = _providers.FirstOrDefault(q => q.ProviderSettings.AllowedCountryCodes.Contains("*"));

        if (providerByCountyCode != null)
            return await providerByCountyCode.SendAsync(to, from, message, cancellationToken);

        var defaultProvider = _providers.FirstOrDefault(q => q.Provider == _settings.DefaultProvider);
        if (defaultProvider != null)
        {
            return await defaultProvider.SendAsync(to, from, message, cancellationToken);
        }

        return false;
    }
}