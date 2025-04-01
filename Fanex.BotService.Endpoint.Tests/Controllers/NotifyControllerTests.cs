namespace Fanex.BotService.Endpoint.Tests.Controllers;

using Core.Models;
using Core.Services.Interfaces;
using Fanex.BotService.Core.Enum;
using Fanex.BotService.EndPoint.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shared.Models;
using Shared.Models.Message;
using Shared.Services.Interfaces;

public class NotifyControllerTests
{
    private readonly NotifyController _notifyController;
    private readonly IConversationService _conversationService;
    private readonly IMessageValidationService _messageLogService;
    private readonly IMessageSenderService _messageSenderService;

    public NotifyControllerTests()
    {
        _conversationService = Substitute.For<IConversationService>();
        _messageLogService = Substitute.For<IMessageValidationService>();
        var commonLogService = Substitute.For<ICommonLogService>();
        _messageSenderService = Substitute.For<IMessageSenderService>();

        _notifyController = new NotifyController(
            commonLogService,
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
        Assert.True(sendMessageResults[0].IsSuccessful);
    }

    [Fact]
    public async Task SendViaSkype_ShouldReturnOkWithResults()
    {
        // Arrange
        var messages = new List<MessageEndpoint>
        {
            new() { MessageId = 1, AppId = 1, ConversationId = "conv1", Content = "Hello" }
        };
        var bots = new List<Bot> { new() { ChannelId = (int)ConversationChannel.Skype, AppId = 1 } };
        _conversationService.GetBotAsync().Returns(bots);
        _messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await _notifyController.SendViaSkype(messages);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResults = okResult?.Value as List<SendMessageResult>;
        Assert.NotNull(sendMessageResults);
        Assert.Single(sendMessageResults);
        Assert.True(sendMessageResults[0].IsSuccessful);
    }

    [Fact]
    public async Task SendViaTelegram_ShouldReturnOkWithResults()
    {
        // Arrange
        var messages = new List<MessageEndpoint>
        {
            new() { MessageId = 1, AppId = 1, ConversationId = "conv1", Content = "Hello" }
        };
        var bots = new List<Bot> { new() { ChannelId = (int)ConversationChannel.Telegram, AppId = 1 } };
        _conversationService.GetBotAsync().Returns(bots);
        _messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await _notifyController.SendViaTelegram(messages);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResults = okResult?.Value as List<SendMessageResult>;
        Assert.NotNull(sendMessageResults);
        Assert.Single(sendMessageResults);
        Assert.True(sendMessageResults[0].IsSuccessful);
    }

    [Fact]
    public async Task SendSingleViaTeams_ShouldReturnOkWithResult()
    {
        // Arrange
        var message = new MessageEndpoint { MessageId = 1, AppId = 1, ConversationId = "conv1", Content = "Hello" };
        var bots = new List<Bot> { new() { ChannelId = (int)ConversationChannel.Teams, AppId = 1 } };
        _conversationService.GetBotAsync().Returns(bots);
        _messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await _notifyController.SendSingleViaTeams(message);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResult = okResult?.Value as SendMessageResult;
        Assert.NotNull(sendMessageResult);
        Assert.True(sendMessageResult.IsSuccessful);
    }

    [Fact]
    public async Task SendSingleViaSkype_ShouldReturnOkWithResult()
    {
        // Arrange
        var message = new MessageEndpoint { MessageId = 1, AppId = 1, ConversationId = "conv1", Content = "Hello" };
        var bots = new List<Bot> { new() { ChannelId = (int)ConversationChannel.Skype, AppId = 1 } };
        _conversationService.GetBotAsync().Returns(bots);
        _messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await _notifyController.SendSingleViaSkype(message);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResult = okResult?.Value as SendMessageResult;
        Assert.NotNull(sendMessageResult);
        Assert.True(sendMessageResult.IsSuccessful);
    }

    [Fact]
    public async Task SendSingleViaTelegram_ShouldReturnOkWithResult()
    {
        // Arrange
        var message = new MessageEndpoint { MessageId = 1, AppId = 1, ConversationId = "conv1", Content = "Hello" };
        var bots = new List<Bot> { new() { ChannelId = (int)ConversationChannel.Telegram, AppId = 1 } };
        _conversationService.GetBotAsync().Returns(bots);
        _messageLogService.IsInvalidSendMessageToBotService(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
        _messageSenderService.SendMessageAsync(Arg.Any<ConversationMessage>()).Returns(new SendMessageResult { ErrorCode = 0 });

        // Act
        var result = await _notifyController.SendSingleViaTelegram(message);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var sendMessageResult = okResult?.Value as SendMessageResult;
        Assert.NotNull(sendMessageResult);
        Assert.True(sendMessageResult.IsSuccessful);
    }

    [Fact]
    public async Task ForwardMessage_ShouldReturnOkWithResult()
    {
        // Arrange
        var conversationMessage = new ConversationMessage
        {
            MessageId = 1,
            ConversationId = "conv1",
            Message = "Hello",
            ServiceUrl = "https://service.url",
            ChannelId = "Telegram",
            ConversationBotId = "bot1",
            BotName = "TestBot"
        };
        var sendMessageResult = new SendMessageResult { ErrorCode = 0 };
        _messageSenderService.SendMessageAsync(conversationMessage).Returns(sendMessageResult);

        // Act
        var result = await _notifyController.ForwardMessage(conversationMessage);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var resultValue = okResult?.Value as SendMessageResult;
        Assert.NotNull(resultValue);
        Assert.True(resultValue.IsSuccessful);
    }
}