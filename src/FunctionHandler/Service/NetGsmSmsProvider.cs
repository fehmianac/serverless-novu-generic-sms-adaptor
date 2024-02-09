using FunctionHandler.Contract;
using FunctionHandler.Enum;
using FunctionHandler.Options;

namespace FunctionHandler.Service;

public class NetGsmSmsProvider : ISmsProvider
{
    private readonly Settings _settings;
    private static HttpClient _httpClient = new HttpClient();

    public NetGsmSmsProvider(Settings settings)
    {
        _settings = settings;
    }

    public Providers Provider => Providers.NetGsm;

    public IProviderSettings ProviderSettings => _settings.Twilio;

    public async Task<bool> SendAsync(string to, string from, string message, CancellationToken cancellationToken)
    {
        var userName = _settings.NetGsm.Username;
        var password = _settings.NetGsm.Password;
        var url =
            $"/sms/send/get/?usercode={userName}&password={password}&gsmno={to}&message={message}&msgheader={from}";

        var request = new HttpRequestMessage(HttpMethod.Get, _settings.NetGsm.BaseUrl + url);
        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine(responseContent);
        return responseContent.StartsWith("00");
    }
}