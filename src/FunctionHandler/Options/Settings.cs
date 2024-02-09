using FunctionHandler.Enum;

namespace FunctionHandler.Options;

public class Settings
{
    public string ApiKey { get; set; } = default!;
    public Providers DefaultProvider { get; set; }
    
    public TwilioSettings Twilio { get; set; } = new();
    public NetGsmSettings NetGsm { get; set; } = new();
    
}