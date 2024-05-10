using FunctionHandler.Contract;
using FunctionHandler.Enum;
using FunctionHandler.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FunctionHandler.Service;

public class TwilioSmsProvider : ISmsProvider
{
    private readonly Settings _settings;
    public TwilioSmsProvider(Settings settings)
    {
        _settings = settings;
    }
    
    public Providers Provider => Providers.Twilio;
    public IProviderSettings ProviderSettings => _settings.Twilio;
    public async Task<bool> SendAsync(string to, string from, string message, CancellationToken cancellationToken)
    {
        var userName = _settings.Twilio.AccountSid;
        var password = _settings.Twilio.AuthToken;

        TwilioClient.Init(username: userName, password: password);

        var fromPhone = new Twilio.Types.PhoneNumber(from);
        if (!string.IsNullOrEmpty(_settings.Twilio.From))
        {
            fromPhone = new Twilio.Types.PhoneNumber(_settings.Twilio.From);
        }
        var twilioMessage = await MessageResource.CreateAsync(
            body: message,
            from: fromPhone,
            to: new Twilio.Types.PhoneNumber(to)
        );
        
        return !string.IsNullOrEmpty(twilioMessage.Sid);
    }
}