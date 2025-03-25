using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services.Interfaces;
using Fanex.BotService.EndPoint.Controllers;
using Fanex.BotService.Shared.Models.Message;
using Fanex.BotService.Shared.Services.Interfaces;
using Fanex.BotService.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Tests.Controllers;

public class NotifyControllerTests
{
    private readonly NotifyController _notifyController;
    private readonly IConversationService _conversationService;
    private readonly IMessageValidationService _messageLogService;
    private readonly ICommonLogService _commonLogService;
    private readonly IMessageSenderService _messageSenderService;

    public NotifyControllerTests()
    {
        _conversationService = Substitute.For<IConversationService>();
        _messageLogService = Substitute.For<IMessageValidationService>();
        _commonLogService = Substitute.For<ICommonLogService>();
        _messageSenderService = Substitute.For<IMessageSenderService>();

        _notifyController = new NotifyController(
            _commonLogService,
            _messageLogService,
            _conversationService,
            _messageSenderService);
    }

    [Fact]
    public async Task Test_ShouldReturnOk()
    {
        // Act
        var result = await _notifyController.Test();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("ok", okResult?.Value);
    }

    [Fact]
    public async Task SendMessage_ShouldReturnOkWithResults()
    {
        // Arrange
        var messages = new List<MessageEndpoint>
        {
            new() { MessageId = 1, AppId = 1, ConversationId = "conv1", Content = "Hello", ChannelId = 1 }
        };
        var bots = new List<Bot> { new() { ChannelId = 1, AppId = 1 } };
        _conversationService.GetBotAsync().Returns(bots);
        _messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await _notifyController.SendMessage(messages);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResults = okResult?.Value as List<SendMessageResult>;
        Assert.NotNull(sendMessageResults);
        Assert.Single(sendMessageResults);
        Assert.True(sendMessageResults[0].IsSucessfull);
    }
}