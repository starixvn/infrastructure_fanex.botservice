using Fanex.BotService.Service.Core.Criteria.Message;
using Fanex.BotService.Service.Core.Enum;
using Fanex.BotService.Service.Core.Models;
using Fanex.BotService.Service.Core.Services;
using Fanex.BotService.Service.Core.Services.Interfaces;
using Fanex.BotService.Shared.Services.Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Fanex.BotService.Core.Tests.Services;

public class MessageServiceTests
{
    private readonly IBaseService baseService;
    private readonly ICommonLogService commonLogService;
    private readonly IMessageQueueService queueMessageService;
    private readonly MessageService messageService;

    public MessageServiceTests()
    {
        baseService = Substitute.For<IBaseService>();
        commonLogService = Substitute.For<ICommonLogService>();
        queueMessageService = Substitute.For<IMessageQueueService>();
        messageService = new MessageService(baseService, commonLogService, queueMessageService);
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
            Images = new List<string> { "image1.png" },
            Channel = BotChannel.Skype
        };
        var commandResult = new CommandResult { Code = 0 };
        baseService.ExecuteNonQueryAsync(Arg.Any<InsertNewMessageCommand>()).Returns(commandResult);

        // Act
        var result = await messageService.InsertNewMessageAsync(conversation, 0);

        // Assert
        await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<InsertNewMessageCommand>());
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
            Images = new List<string> { "image1.png" },
            Channel = BotChannel.Skype
        };
        baseService.ExecuteNonQueryAsync(Arg.Any<InsertNewMessageCommand>()).Throws(new Exception("Database error"));

        // Act
        var result = await messageService.InsertNewMessageAsync(conversation, 0);

        // Assert
        await commonLogService.Received(1).SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
        await queueMessageService.Received(1).SchedulerPublishAsync(conversation, 10);
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
        baseService.ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>()).Returns(commandResult);

        // Act
        await messageService.UpdateMessageStatusAsync(result, 0);

        // Assert
        await baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>());
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
        baseService.ExecuteNonQueryAsync(Arg.Any<UpdateMessageStatusCommand>()).Throws(new Exception("Database error"));

        // Act
        await messageService.UpdateMessageStatusAsync(result, 0);

        // Assert
        await commonLogService.Received(1).SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
        await queueMessageService.Received(1).SchedulerPublishAsync(result, 10);
    }
}