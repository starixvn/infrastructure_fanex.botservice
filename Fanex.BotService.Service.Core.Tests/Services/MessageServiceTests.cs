using Fanex.BotService.Service.Core.Criteria.Message;
using Fanex.BotService.Service.Core.Enum;
using Fanex.BotService.Service.Core.Models;
using Fanex.BotService.Service.Core.Services;
using Fanex.BotService.Service.Core.Services.Interfaces;
using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Fanex.BotService.Core.Tests.Services;

public class MessageServiceTests
{
    private readonly IBaseService _baseService;
    private readonly ICommonLogService _commonLogService;
    private readonly IMessageQueueService _queueMessageService;
    private readonly MessageService _messageService;

    public MessageServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _commonLogService = Substitute.For<ICommonLogService>();
        _queueMessageService = Substitute.For<IMessageQueueService>();
        _messageService = new MessageService(_baseService, _commonLogService, _queueMessageService);
    }

    [Fact]
    public async Task InsertNewMessageAsync_ShouldInsertMessage()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Hello",
            Images = ["image1.png"],
            Channel = BotChannel.Skype
        };
        var commandResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<InsertNewMessageCommand>()).Returns(commandResult);

        // Act
        var result = await _messageService.InsertNewMessageAsync(conversation, 0);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<InsertNewMessageCommand>());
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task InsertNewMessageAsync_ShouldLogErrorAndScheduleRetry_OnException()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Hello",
            Images = ["image1.png"],
            Channel = BotChannel.Skype
        };
        _baseService.ExecuteNonQueryAsync(Arg.Any<InsertNewMessageCommand>()).Throws(new Exception("Database error"));

        // Act
        var result = await _messageService.InsertNewMessageAsync(conversation, 0);

        // Assert
        await _commonLogService.Received(1).SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
        await _queueMessageService.Received(1).SchedulerPublishAsync(conversation, 10);
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task UpdateMessageStatusAsync_ShouldUpdateStatus()
    {
        // Arrange
        var result = new SendMessageResult
        {
            MessageId = 1,
            ErrorCode = 0
        };
        var commandResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>()).Returns(commandResult);

        // Act
        await _messageService.UpdateMessageStatusAsync(result, 0);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>());
    }

    [Fact]
    public async Task UpdateMessageStatusAsync_ShouldLogError_WhenCommandResultIsNotSuccessful()
    {
        // Arrange
        var result = new SendMessageResult
        {
            MessageId = 1,
            ErrorCode = 0,
        };
        var commandResult = new CommandResult
        {
            Code = 1,
            Message = "Failed to update status"
        };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>()).Returns(commandResult);

        // Act
        await _messageService.UpdateMessageStatusAsync(result, 0);

        // Assert
        await _commonLogService.Received(1).SentryErrorAsync(commandResult.Message, Arg.Is<Exception>(ex => ex.Message == commandResult.Message));
    }

    [Fact]
    public async Task UpdateMessageStatusAsync_ShouldLogErrorAndScheduleRetry_OnException()
    {
        // Arrange
        var result = new SendMessageResult
        {
            MessageId = 1,
            ErrorCode = 1
        };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>()).Throws(new Exception("Database error"));

        // Act
        await _messageService.UpdateMessageStatusAsync(result, 0);

        // Assert
        await _commonLogService.Received(1).SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
        await _queueMessageService.Received(1).SchedulerPublishAsync(result, 10);
    }
}