using System.Text.Json.Serialization;

namespace FunctionHandler.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Providers
{
    NetGsm,
    Twilio
}