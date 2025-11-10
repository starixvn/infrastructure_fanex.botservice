namespace Fanex.BotService.Consumer.MessageBrokers;

using EasyNetQ;
using Fanex.BotService.Service.Core.Enum;
using Newtonsoft.Json;
using Service.Core.Configurations;
using Service.Core.Constants;
using Service.Core.Models;
using Service.Core.Services.Interfaces;
using Shared.Models;
using Shared.Models.Message;
using Shared.Services.Interfaces;

public interface IMessageBroker : IDisposable
{
    void Start();
}

public class MessageBroker(IBus bus,
    ICommonLogService logService,
    IMessageService messageService,
    IBotClientService botClientService,
    IMessageQueueService messageQueueService) : IMessageBroker
{
    private readonly List<ConsumerSetting> _consumerSettings = AppSettings.ConsumerSettings;
    private const int TimeDelayBeforeRetry = 10;
    private const int MaxRetryTime = 5;

    public void Start()
    {
        InsertMessageToDatabaseConsumer();
        SendMessageToEndpointConsumer();
        UpdateMessageStatusConsumer();
    }

    public void InsertMessageToDatabaseConsumer()
    {
        var setting = GetConsumerSetting(QueueSubscriptionId.InsertFromQueueToDatabase);
        if (setting is not null && setting.IsConsumed)
        {
            Subscribe<Conversation>(QueueSubscriptionId.InsertFromQueueToDatabase,
                async void (message) =>
                {
                    try
                    {
                        logService.LogFileMessage($"Begin consume: {QueueSubscriptionId.InsertFromQueueToDatabase}. Data: {JsonConvert.SerializeObject(message)}");

                        var messageId = await messageService.InsertNewMessageAsync(message, setting.RetryTimeSpan ?? 1);
                        if (messageId > 0)
                        {
                            MessageEndpoint msgSendToEndpoint = new()
                            {
                                MessageId = messageId,
                                AppId = message.AppId,
                                ChannelId = (byte)message.Channel,
                                ConversationId = message.ConversationId,
                                Content = message.Content,
                                Images = message.Images,
                            };
                            await messageQueueService.PublishAsync(msgSendToEndpoint);
                        }

                        logService.LogFileMessage($"End consume {QueueSubscriptionId.InsertFromQueueToDatabase}");
                    }
                    catch (Exception e)
                    {
                        await logService.SentryErrorAsync(e.Message, e);
                    }
                });
        }
    }

    public void SendMessageToEndpointConsumer()
    {
        var setting = GetConsumerSetting(QueueSubscriptionId.SendMessageToBotServiceEndpoint);
        if (setting is not null && setting.IsConsumed)
        {
            Subscribe<MessageEndpoint>(QueueSubscriptionId.SendMessageToBotServiceEndpoint,
                async void (message) =>
                {
                    try
                    {
                        logService.LogFileMessage($"Begin consume: {QueueSubscriptionId.SendMessageToBotServiceEndpoint}. Data: {JsonConvert.SerializeObject(message)}");

                        var sentResult = await botClientService.SendMessageAsync((BotChannel)message.ChannelId, message);

                        if (!sentResult.IsSuccessful)
                        {
                            await logService.SentryErrorAsync($"Send message to bot service endpoint failed. MessageId: {message.MessageId}, Error: {sentResult.ErrorMessage}");
                        }
                        await messageQueueService.PublishAsync(sentResult);

                        logService.LogFileMessage($"End consume {QueueSubscriptionId.SendMessageToBotServiceEndpoint}");
                    }
                    catch (Exception e)
                    {
                        await logService.SentryErrorAsync(e.Message, e);
                    }
                });
        }
    }

    public void UpdateMessageStatusConsumer()
    {
        var setting = GetConsumerSetting(QueueSubscriptionId.UpdateMessageStatus);
        if (setting is not null && setting.IsConsumed)
        {
            Subscribe<SendMessageResult>(QueueSubscriptionId.UpdateMessageStatus,
                async void (message) =>
                {
                    try
                    {
                        logService.LogFileMessage($"Begin consume: {QueueSubscriptionId.UpdateMessageStatus}. Data: {JsonConvert.SerializeObject(message)}");

                        await messageService.UpdateMessageStatusAsync(message, setting.RetryTimeSpan ?? 1);

                        logService.LogFileMessage($"End consume {QueueSubscriptionId.UpdateMessageStatus}");
                    }
                    catch (Exception e)
                    {
                        await logService.SentryErrorAsync(e.Message, e);
                    }
                });
        }
    }

    private ConsumerSetting? GetConsumerSetting(string queueName)
        => _consumerSettings.Find(s => s.QueueName.Equals(queueName, StringComparison.OrdinalIgnoreCase));

    #region Subscribe

    private void Subscribe<T>(string subscriptionId, Action<T> action, int retryCount = 0)
    {
        var isFailed = false;

        try
        {
            bus.PubSub.SubscribeAsync(
                subscriptionId,
                action);

            logService.LogFileMessage($"Subscribe the queue with key {subscriptionId} successfully");
        }
        catch (Exception ex)
        {
            logService.SentryErrorAsync($"There is an error while subscribing the queue with key {subscriptionId}. Retrying...", ex);
            isFailed = true;
        }

        if (isFailed && retryCount < MaxRetryTime)
        {
            Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetry));
            Subscribe(subscriptionId, action, retryCount);
        }
    }

    #endregion Subscribe

    #region IDisposable Support

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                //  dispose managed state (managed objects).
            }

            bus?.Dispose();

            _disposedValue = true;
        }
    }

    ~MessageBroker()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion IDisposable Support
}