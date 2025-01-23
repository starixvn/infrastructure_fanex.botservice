namespace Fanex.BotService.Consumer.MessageBrokers
{
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
        private readonly List<ConsumerSetting> consumerSettings = AppSettings.ConsumerSettings;
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
            var setting = GetConsumerSetting(QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE);
            if (setting is not null && setting.IsConsumed)
            {
                Subscribe<Conversation>(QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE,
                async (message) =>
                {
                    logService.LogFileMessage($"Begin consume: {QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE}. Data: {JsonConvert.SerializeObject(message)}");

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

                    logService.LogFileMessage($"End consume {QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE}");
                });
            }
        }

        public void SendMessageToEndpointConsumer()
        {
            var setting = GetConsumerSetting(QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT);
            if (setting is not null && setting.IsConsumed)
            {
                Subscribe<MessageEndpoint>(QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT,
                async (message) =>
                {
                    logService.LogFileMessage($"Begin consume: {QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT}. Data: {JsonConvert.SerializeObject(message)}");

                    var sentResult = await botClientService.SendMessageAsync((BotChannel)message.ChannelId, message);

                    await messageQueueService.PublishAsync(sentResult);

                    logService.LogFileMessage($"End consume {QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT}");
                });
            }
        }

        public void UpdateMessageStatusConsumer()
        {
            var setting = GetConsumerSetting(QueueSubcriptionId.UPDATE_MESSAGE_STATUS);
            if (setting is not null && setting.IsConsumed)
            {
                Subscribe<SendMessageResult>(QueueSubcriptionId.UPDATE_MESSAGE_STATUS,
                async (message) =>
                {
                    logService.LogFileMessage($"Begin consume: {QueueSubcriptionId.UPDATE_MESSAGE_STATUS}. Data: {JsonConvert.SerializeObject(message)}");

                    await messageService.UpdateMessageStatusAsync(message, setting.RetryTimeSpan ?? 1);

                    logService.LogFileMessage($"End consume {QueueSubcriptionId.UPDATE_MESSAGE_STATUS}");
                });
            }
        }

        private ConsumerSetting? GetConsumerSetting(string queueName)
            => consumerSettings.Find(s => s.QueueName.Equals(queueName, StringComparison.OrdinalIgnoreCase));

        #region Subscribe

        private void Subscribe<T>(string subscriptionId, Action<T> action, int retryCount = 0)
        {
            bool isFailed = false;

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

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //  dispose managed state (managed objects).
                }

                bus?.Dispose();

                disposedValue = true;
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
}