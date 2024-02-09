using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FunctionHandler.Contract;
using FunctionHandler.Model;
using FunctionHandler.Options;
using FunctionHandler.Service;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FunctionHandler;

public class Entrypoint
{
    private static ISmsSender? _smsSender = null;
    private static Settings? _settings = null;
    private static IJsonService? _jsonService = null;
    private static IParameterService? _parameterService = null;


    public Entrypoint()
    {
        _jsonService ??= new JsonService();
        _parameterService ??= new ParameterService(new AmazonSimpleSystemsManagementClient());
    }

    public Entrypoint(ISmsSender smsSender, IJsonService jsonService,
        IParameterService parameterService)
    {
        _smsSender = smsSender;
        _jsonService = jsonService;
        _parameterService = parameterService;
    }

    public async Task<APIGatewayHttpApiV2ProxyResponse> Handler(APIGatewayHttpApiV2ProxyRequest request,
        ILambdaContext context)
    {
        Console.WriteLine(JsonSerializer.Serialize(request));
        var apiKey = "";
        if (request.Headers.TryGetValue("x-api-key", out var value))
        {
            apiKey = value;
        }

        if (string.IsNullOrEmpty(apiKey))
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 403,
                Body = "Forbidden"
            };
        }

        if (request.Body == null)
        {
            Console.WriteLine("Response Api Key");
            var responsePayload = JsonSerializer.Serialize(new ApiKeyResponse
            {
                ApiKey = apiKey
            });
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = responsePayload,
            };
        }

        _settings ??= await _parameterService.GetSettingsAsync();
        _smsSender ??= new SmsSender(_settings);

        var apiKeyIsValid = _smsSender.ValidateApiKey(apiKey);
        if (!apiKeyIsValid)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 403,
                Body = "Forbidden"
            };
        }

        var message = _jsonService.DeserializeAsync<RequestModel>(request.Body);

        if (message == null)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 400,
                Body = "Invalid request"
            };
        }

        var smsResponse = await _smsSender.SendAsync(message.To, message.From, message.Content);
        if (!smsResponse)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 500,
                Body = "Failed to send SMS"
            };
        }

        var responseBody = new ResponseModel
        {
            Id = Guid.NewGuid().ToString(),
            Date = DateTime.UtcNow.ToString("O")
        };


        var response = new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(responseBody)
        };
        return response;
    }
}