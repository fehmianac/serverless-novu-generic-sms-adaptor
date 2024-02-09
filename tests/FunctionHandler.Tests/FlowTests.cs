using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Autofac.Extras.Moq;
using AutoFixture;
using FunctionHandler.Contract;
using FunctionHandler.Model;
using FunctionHandler.Options;
using FunctionHandler.Service;
using Moq;
using Xunit;

namespace FunctionHandler.Tests;

public class FlowTests
{
    private readonly IFixture _fixture;
    private readonly AutoMock _autoMock;

    public FlowTests()
    {
        _fixture = new Fixture();
        _autoMock = AutoMock.GetStrict();
    }

    [Fact]
    public async Task When_Api_Key_Null_Return_403()
    {
        // Arrange
        var entryPoint = new Entrypoint();
        var response = await entryPoint.Handler(new APIGatewayHttpApiV2ProxyRequest
        {
            Headers = new Dictionary<string, string>()
        }, null);
        // Assert

        Assert.Equal(403, response.StatusCode);
        Assert.Equal("Forbidden", response.Body);
    }

    [Fact]
    public async Task? When_Api_Key_Is_Not_Valid_Return_403()
    {
        
        var smsSenderMock = _autoMock.Mock<ISmsSender>();
        var jsonServiceMock = _autoMock.Mock<IJsonService>();
        var parameterServiceMock = _autoMock.Mock<IParameterService>();
        var settings = new Settings();
        parameterServiceMock.Setup(q => q.GetSettingsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);
        smsSenderMock.Setup(q => q.ValidateApiKey(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(false);
        // Arrange
        var entryPoint = new Entrypoint(smsSenderMock.Object, jsonServiceMock.Object, parameterServiceMock.Object);
        var response = await entryPoint.Handler(new APIGatewayHttpApiV2ProxyRequest
        {
            Headers = new Dictionary<string, string>
            {
                { "x-api-key", "invalid-key" }
            }
        }, null);
        // Assert

        Assert.Equal(403, response.StatusCode);
        Assert.Equal("Forbidden", response.Body);
    }
    
    [Fact]
    public async Task? When_Body_Is_Not_Valid_Return_400()
    {
        var smsSenderMock = _autoMock.Mock<ISmsSender>();
        var jsonServiceMock = _autoMock.Mock<IJsonService>();
        var parameterServiceMock = _autoMock.Mock<IParameterService>();
        var settings = new Settings();
        parameterServiceMock.Setup(q => q.GetSettingsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);
        smsSenderMock.Setup(q => q.ValidateApiKey(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(true);
        jsonServiceMock.Setup(q => q.DeserializeAsync<RequestModel>(It.IsAny<string>())).Returns((RequestModel)null);
        
        // Arrange
        var entryPoint = new Entrypoint(smsSenderMock.Object, jsonServiceMock.Object, parameterServiceMock.Object);
        var response = await entryPoint.Handler(new APIGatewayHttpApiV2ProxyRequest
        {
            Headers = new Dictionary<string, string>
            {
                { "x-api-key", "invalid-key" }
            }
        }, null);
        // Assert

        Assert.Equal(400, response.StatusCode);
        Assert.Equal("Invalid request", response.Body);
    }

    [Fact]
    public async Task? When_Send_Sms_Failed_Return_500()
    {

        var message = _fixture.Create<RequestModel>();
        var smsSenderMock = _autoMock.Mock<ISmsSender>();
        var parameterServiceMock = _autoMock.Mock<IParameterService>();
        var settings = _fixture.Create<Settings>();
        parameterServiceMock.Setup(q => q.GetSettingsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);
        smsSenderMock.Setup(q => q.ValidateApiKey(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(true);
        smsSenderMock
            .Setup(q => q.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(false);
        // Arrange
        var entryPoint = new Entrypoint(smsSenderMock.Object, new JsonService(), parameterServiceMock.Object);
        var response = await entryPoint.Handler(new APIGatewayHttpApiV2ProxyRequest
        {
            Headers = new Dictionary<string, string>
            {
                { "x-api-key", settings.ApiKey }
            },
            Body = JsonSerializer.Serialize(message)
        }, null);
        // Assert

        Assert.Equal(500, response.StatusCode);
        Assert.Equal("Failed to send SMS", response.Body);
    }

    [Fact]
    public async Task? When_Send_Sms_Success_Return_200()
    {
        var message = _fixture.Create<RequestModel>();
        var smsSenderMock = _autoMock.Mock<ISmsSender>();
        var parameterServiceMock = _autoMock.Mock<IParameterService>();
        var settings = _fixture.Create<Settings>();
        parameterServiceMock.Setup(q => q.GetSettingsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(settings);
        smsSenderMock.Setup(q => q.ValidateApiKey(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(true);
        smsSenderMock
            .Setup(q => q.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(true);
        // Arrange
        var entryPoint = new Entrypoint(smsSenderMock.Object, new JsonService(), parameterServiceMock.Object);
        var response = await entryPoint.Handler(new APIGatewayHttpApiV2ProxyRequest
        {
            Headers = new Dictionary<string, string>
            {
                { "x-api-key", settings.ApiKey }
            },
            Body = JsonSerializer.Serialize(message)
        }, null);
        // Assert

        var responseBody = JsonSerializer.Deserialize<ResponseModel>(response.Body);
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(responseBody);
        Assert.NotNull(responseBody.Id);
        Assert.NotNull(responseBody.Date);
    }
}