using Fanex.BotService.Core;
using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services.Interfaces;
using Fanex.BotService.EndPoint.Controllers;
using Fanex.BotService.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AuthController _authController;
    private readonly IConversationService _conversationService;
    private readonly IMessageSenderService _messageSenderService;

    public AuthControllerTests()
    {
        var appSettings = new AppSettings
        {
            AppCredentialId = "testUser",
            AppCredentialPassword = "testPassword",
            Secret = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ",
            JwtLifeTime = 30
        };

        var options = Substitute.For<IOptions<AppSettings>>();
        options.Value.Returns(appSettings);

        _conversationService = Substitute.For<IConversationService>();
        _messageSenderService = Substitute.For<IMessageSenderService>();

        _authController = new AuthController(options.Value, _conversationService, _messageSenderService);
    }

    [Fact]
    public void Authenticate_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var username = "testUser";
        var password = "testPassword";

        // Act
        var result = _authController.Authenticate(username, password) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Authenticate_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var username = "invalidUser";
        var password = "invalidPassword";

        // Act
        var result = _authController.Authenticate(username, password) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Username or password is incorrect", result.Value);
    }

    [Fact]
    public async Task TestBot_ReturnsOkResult()
    {
        // Arrange
        var bots = new List<Bot>
            {
                new Bot
                {
                    BotId = 1,
                    ConversationId = "1064787034",
                    BotName = "TestBot",
                    ChannelId = 2,
                    ChannelName = "Telegram",
                    ServiceUrl = "https://service.url",
                    AppId = 1018,
                    MicrosoftAppId = "MicrosoftAppId"
                }
            };

        _conversationService.GetBotAsync().Returns(bots);

        var sendMessageResult = new SendMessageResult
        {
            MessageId = 1,
            ConversationId = "1064787034",
            SentTime = DateTime.UtcNow,
            ErrorCode = 0,
            ErrorMessage = null,
            IsIgnoreRetry = false
        };

        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(sendMessageResult);

        // Act
        var result = await _authController.TestBot() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sendMessageResult, result.Value);
    }
}