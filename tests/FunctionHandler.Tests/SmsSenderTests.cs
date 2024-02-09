using Autofac.Extras.Moq;
using AutoFixture;
using FunctionHandler.Contract;
using FunctionHandler.Enum;
using FunctionHandler.Options;
using FunctionHandler.Service;
using Moq;
using Xunit;

namespace FunctionHandler.Tests;

public class SmsSenderTests
{
    private readonly IFixture _fixture;
    private readonly AutoMock _autoMock;

    public SmsSenderTests()
    {
        _fixture = new Fixture();
        _autoMock = AutoMock.GetStrict();
    }

    [Fact]
    public void Check_ApiKey_If_Equals_Should_Return_True()
    {
        var settings = _fixture.Create<Settings>();
        var smsSender = new SmsSender(settings);
        var result = smsSender.ValidateApiKey(settings.ApiKey);
        Assert.True(result);
    }

    [Fact]
    public void Check_ApiKey_If_Not_Equals_Should_Return_False()
    {
        var settings = _fixture.Create<Settings>();
        var smsSender = new SmsSender(settings);
        var result = smsSender.ValidateApiKey(settings.ApiKey + "122");
        Assert.False(result);
    }

    [Fact]
    public async Task Should_Use_NetGsm_When_Country_Code_Is_TR()
    {
        var to = "+905555555555";
        var from = "from";
        var message = "message";
        var settings = _fixture.Create<Settings>();
        settings.NetGsm.AllowedCountryCodes.Add("90");
        var mockNetGsm = _autoMock.Mock<ISmsProvider>();
        mockNetGsm.Setup(q => q.Provider).Returns(Providers.NetGsm);
        mockNetGsm.Setup(q => q.ProviderSettings.AllowedCountryCodes).Returns(new List<string> { "90" });
        mockNetGsm.Setup(q => q.SendAsync(to, from, message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var smsSender = new SmsSender(settings, new List<ISmsProvider>
        {
            mockNetGsm.Object,
            new TwilioSmsProvider(settings)
        });
        var result = await smsSender.SendAsync(to, from, message);
        Assert.True(result);
        mockNetGsm.Verify(q => q.SendAsync(to, from, message, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Should_Use_Twilio_When_Country_Code_Is_Not_TR()
    {
        var to = "+105555555555";
        var from = "from";
        var message = "message";
        var settings = _fixture.Create<Settings>();
        var mockTwilio = _autoMock.Mock<ISmsProvider>();
        mockTwilio.Setup(q => q.Provider).Returns(Providers.Twilio);
        mockTwilio.Setup(q => q.ProviderSettings.AllowedCountryCodes).Returns(new List<string> { "1" });
        mockTwilio.Setup(q => q.SendAsync(to, from, message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var smsSender = new SmsSender(settings, new List<ISmsProvider>
        {
            mockTwilio.Object,
            new NetGsmSmsProvider(settings)
        });
        var result = await smsSender.SendAsync(to, from, message);
        Assert.True(result);
        mockTwilio.Verify(q => q.SendAsync(to, from, message, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Should_Use_Default_When_Country_Not_Configured()
    {
        var to = "+205555555555";
        var from = "from";
        var message = "message";
        var settings = _fixture.Create<Settings>();
        settings.DefaultProvider = Providers.Twilio;
        var mockTwilio = _autoMock.Mock<ISmsProvider>();
        mockTwilio.Setup(q => q.Provider).Returns(Providers.Twilio);
        mockTwilio.Setup(q => q.ProviderSettings.AllowedCountryCodes).Returns(new List<string> { "1" });
        mockTwilio.Setup(q => q.SendAsync(to, from, message, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var smsSender = new SmsSender(settings, new List<ISmsProvider>
        {
            mockTwilio.Object,
            new NetGsmSmsProvider(settings)
        });
        var result = await smsSender.SendAsync(to, from, message);
        Assert.True(result);
        mockTwilio.Verify(q => q.SendAsync(to, from, message, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task? Should_Return_False_If_Miss_Configured()
    {
        var to = "+205555555555";
        var from = "from";
        var message = "message";
        var settings = _fixture.Create<Settings>();
        settings.DefaultProvider = Providers.Twilio;

        var smsSender = new SmsSender(settings, new List<ISmsProvider>
        {
            new NetGsmSmsProvider(settings)
        });
        var result = await smsSender.SendAsync(to, from, message);
        Assert.False(result);
    }
    
    [Fact]
    public async Task? Exception_There_Is_No_Active_Provider()
    {
        var to = "+205555555555";
        var from = "from";
        var message = "message";
        var settings = _fixture.Create<Settings>();
        settings.NetGsm.IsEnabled = false;
        settings.Twilio.IsEnabled = false;

        Assert.Throws<InvalidOperationException>(() =>
        {
            var smsSender = new SmsSender(settings);
        });
    }
    
}