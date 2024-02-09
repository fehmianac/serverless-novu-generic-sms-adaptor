using FunctionHandler.Contract;

namespace FunctionHandler.Options;

public class NetGsmSettings : IProviderSettings
{
    public string BaseUrl { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool IsEnabled { get; set; }
    public List<string> AllowedCountryCodes { get; set; } = new();
}