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
    private readonly NotifyController notifyController;
    private readonly IConversationService conversationService;
    private readonly IMessageValidationService messageLogService;
    private readonly ICommonLogService commonLogService;
    private readonly IMessageSenderService messageSenderService;

    public NotifyControllerTests()
    {
        conversationService = Substitute.For<IConversationService>();
        messageLogService = Substitute.For<IMessageValidationService>();
        commonLogService = Substitute.For<ICommonLogService>();
        messageSenderService = Substitute.For<IMessageSenderService>();

        notifyController = new NotifyController(
            commonLogService,
            messageLogService,
            conversationService,
            messageSenderService);
    }

    [Fact]
    public async Task Test_ShouldReturnOk()
    {
        // Act
        var result = await notifyController.Test();

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
        conversationService.GetBotAsync().Returns(bots);
        messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await notifyController.SendMessage(messages);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResults = okResult?.Value as List<SendMessageResult>;
        Assert.NotNull(sendMessageResults);
        Assert.Single(sendMessageResults);
        Assert.True(sendMessageResults[0].IsSucessfull);
    }
}