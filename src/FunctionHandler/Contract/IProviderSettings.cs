namespace FunctionHandler.Contract;

public interface IProviderSettings
{
    public bool IsEnabled { get; set; }
    List<string> AllowedCountryCodes { get; set; }
}