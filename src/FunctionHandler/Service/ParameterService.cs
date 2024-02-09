using System.Text.Json;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FunctionHandler.Contract;
using FunctionHandler.Options;

namespace FunctionHandler.Service;

public class ParameterService : IParameterService
{
    private readonly IAmazonSimpleSystemsManagement _client;

    public ParameterService(AmazonSimpleSystemsManagementClient client)
    {
        _client = client;
    }

    public async Task<Settings> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var parameter = await _client.GetParameterAsync(new GetParameterRequest
        {
            Name = "/generic-sms-adapter/Settings"
        }, cancellationToken);

        return JsonSerializer.Deserialize<Settings>(parameter.Parameter.Value);
    }
}