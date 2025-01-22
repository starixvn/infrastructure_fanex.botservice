using EasyNetQ;
using Fanex.BotService.Service.Core.Configurations;
using Fanex.BotService.Service.Core.Constants;
using Fanex.BotService.Service.Core.Enum;
using Fanex.BotService.Service.Core.Models;
using Fanex.BotService.Service.Core.Services.Interfaces;
using Fanex.BotService.Shared.Models.Message;
using Fanex.BotService.Shared.Services.Interfaces;
using Fanex.BotService.Shared.Models;
using NSubstitute;

namespace Fanex.BotService.Consumer.Tests.MessageBroker;

public class MessageBrokerTests
{
    private readonly IBus bus;
    private readonly ICommonLogService logService;
    private readonly IMessageService messageService;
    private readonly IBotClientService botClientService;
    private readonly IMessageQueueService messageQueueService;
    private readonly MessageBrokers.MessageBroker messageBroker;

    public MessageBrokerTests()
    {
        bus = Substitute.For<IBus>();
        logService = Substitute.For<ICommonLogService>();
        messageService = Substitute.For<IMessageService>();
        botClientService = Substitute.For<IBotClientService>();
        messageQueueService = Substitute.For<IMessageQueueService>();
        AppSettings.ConsumerSettings = new List<ConsumerSetting>
        {
            new() { QueueName = QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE, IsConsumed = true, RetryTimeSpan = 1 },
            new() { QueueName = QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT, IsConsumed = true, RetryTimeSpan = 1 },
            new() { QueueName = QueueSubcriptionId.UPDATE_MESSAGE_STATUS, IsConsumed = true, RetryTimeSpan = 1 }
        };
        messageBroker = new MessageBrokers.MessageBroker(bus, logService, messageService, botClientService, messageQueueService);
    }

    [Fact]
    public void Start_ShouldCallConsumers()
    {
        // Act
        messageBroker.Start();

        // Assert
        Assert.Equal(3, bus.PubSub.ReceivedCalls().Count());
    }

    [Fact]
    public void InsertMessageToDatabaseConsumer_ShouldInsertMessageAndPublish()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Test",
            Channel = BotChannel.Skype,
            Images = new List<string>()
        };
        var messageId = 1;
        messageService.InsertNewMessageAsync(conversation, Arg.Any<int>()).Returns(messageId);

        // Act
        messageBroker.InsertMessageToDatabaseConsumer();

        // Assert
        Assert.Single(bus.PubSub.ReceivedCalls());
    }

    [Fact]
    public void SendMessageToEndpointConsumer_ShouldSendMessageAndPublishResult()
    {
        // Arrange
        var messageEndpoint = new MessageEndpoint { MessageId = 1, AppId = 1, ConversationId = "123", Content = "Test", ChannelId = (byte)BotChannel.Skype, Images = new List<string>() };
        var sendMessageResult = new SendMessageResult { MessageId = 1, ConversationId = "123", SentTime = DateTime.Now, ErrorCode = 0, ErrorMessage = "", IsIgnoreRetry = false };
        botClientService.SendMessageAsync(BotChannel.Skype, messageEndpoint).Returns(sendMessageResult);

        // Act
        messageBroker.SendMessageToEndpointConsumer();

        // Assert
        Assert.Single(bus.PubSub.ReceivedCalls());
    }

    [Fact]
    public void UpdateMessageStatusConsumer_ShouldUpdateMessageStatus()
    {
        // Arrange

        // Act
        messageBroker.UpdateMessageStatusConsumer();

        // Assert
        Assert.Single(bus.PubSub.ReceivedCalls());
    }
}