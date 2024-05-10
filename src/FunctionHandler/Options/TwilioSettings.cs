using FunctionHandler.Contract;

namespace FunctionHandler.Options;

public class TwilioSettings : IProviderSettings
{
    public string AccountSid { get; set; } = default!;
    public string AuthToken { get; set; } = default!;
    public bool IsEnabled { get; set; }
    public string? From { get; set; }
    public List<string> AllowedCountryCodes { get; set; } = new();
}