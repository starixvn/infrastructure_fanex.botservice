using EasyNetQ;
using Fanex.BotService.Service.Core.Configurations;
using Fanex.BotService.Service.Core.Constants;
using Fanex.BotService.Service.Core.Enum;
using Fanex.BotService.Service.Core.Models;
using Fanex.BotService.Service.Core.Services.Interfaces;
using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Models.Message;
using Fanex.BotService.Shared.Services.Interfaces;
using NSubstitute;

namespace Fanex.BotService.Consumer.Tests.MessageBroker;

public class MessageBrokerTests
{
    private readonly IBus _bus;
    private readonly ICommonLogService _logService;
    private readonly IMessageService _messageService;
    private readonly IBotClientService _botClientService;
    private readonly IMessageQueueService _messageQueueService;
    private readonly MessageBrokers.MessageBroker _messageBroker;

    public MessageBrokerTests()
    {
        _bus = Substitute.For<IBus>();
        _logService = Substitute.For<ICommonLogService>();
        _messageService = Substitute.For<IMessageService>();
        _botClientService = Substitute.For<IBotClientService>();
        _messageQueueService = Substitute.For<IMessageQueueService>();
        AppSettings.ConsumerSettings =
        [
            new() { QueueName = QueueSubscriptionId.InsertFromQueueToDatabase, IsConsumed = true, RetryTimeSpan = 1 },
            new()
            {
                QueueName = QueueSubscriptionId.SendMessageToBotServiceEndpoint, IsConsumed = true, RetryTimeSpan = 1
            },
            new() { QueueName = QueueSubscriptionId.UpdateMessageStatus, IsConsumed = true, RetryTimeSpan = 1 }
        ];
        _messageBroker = new MessageBrokers.MessageBroker(_bus, _logService, _messageService, _botClientService, _messageQueueService);
    }

    [Fact]
    public void Start_ShouldCallConsumers()
    {
        // Act
        _messageBroker.Start();

        // Assert
        Assert.Equal(3, _bus.PubSub.ReceivedCalls().Count());
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
            Images = []
        };
        var messageId = 1;
        _messageService.InsertNewMessageAsync(conversation, Arg.Any<int>()).Returns(messageId);

        // Act
        _messageBroker.InsertMessageToDatabaseConsumer();

        // Assert
        Assert.Single(_bus.PubSub.ReceivedCalls());
    }

    [Fact]
    public void SendMessageToEndpointConsumer_ShouldSendMessageAndPublishResult()
    {
        // Arrange
        var messageEndpoint = new MessageEndpoint
        {
            MessageId = 1,
            AppId = 1,
            ConversationId = "123",
            Content = "Test",
            ChannelId = (byte)BotChannel.Skype,
            Images =
            []
        };
        var sendMessageResult = new SendMessageResult { MessageId = 1, ConversationId = "123", SentTime = DateTime.Now, ErrorCode = 0, ErrorMessage = "", IsIgnoreRetry = false };
        _botClientService.SendMessageAsync(BotChannel.Skype, messageEndpoint).Returns(sendMessageResult);

        // Act
        _messageBroker.SendMessageToEndpointConsumer();

        // Assert
        Assert.Single(_bus.PubSub.ReceivedCalls());
    }

    [Fact]
    public void UpdateMessageStatusConsumer_ShouldUpdateMessageStatus()
    {
        // Arrange

        // Act
        _messageBroker.UpdateMessageStatusConsumer();

        // Assert
        Assert.Single(_bus.PubSub.ReceivedCalls());
    }
}