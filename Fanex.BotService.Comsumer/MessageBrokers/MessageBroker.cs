namespace Fanex.BotService.Comsumer.MessageBrokers
{
    using EasyNetQ;
    using Fanex.BotService.Service.Core.Configurations;
    using Fanex.BotService.Service.Core.Constants;
    using Fanex.BotService.Service.Core.Enum;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Newtonsoft.Json;

    public interface IMessageBroker : IDisposable
    {
        void Start();
    }

    public class MessageBroker : IMessageBroker
    {
        private readonly IBus bus;
        private readonly ICommonLogService logService;
        private readonly IMessageService messageService;
        private readonly IBotClientService botClientService;
        private readonly IMessageQueueService messageQueueService;
        private readonly List<ConsumerSetting> consumerSettings;
        private readonly int TimeDelayBeforeRetryInSeccond = 10;
        private readonly int MaxRetryTime = 5;

        public MessageBroker(IBus bus,
             ICommonLogService logService,
             IMessageService messageService,
             IBotClientService botClientService,
             IMessageQueueService messageQueueService)
        {
            this.bus = bus;
            this.logService = logService;
            this.messageService = messageService;
            this.botClientService = botClientService;
            this.messageQueueService = messageQueueService;
            consumerSettings = AppSettings.ConsumerSettings;
        }

        public void Start()
        {
            InsertMessageToDatabaseConsumer();
            SendMessageToEndpointConsumer();
            UpdateMessageStatusConsumer();
        }

        private void InsertMessageToDatabaseConsumer()
        {
            var setting = GetConsumerSetting(QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE);
            if (setting is not null && setting.IsConsumed)
            {
                Subscribe<Conversation>(QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE,
                async (message) =>
                {
                    logService.LogFileMessage($"Begin consum: {QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE}. Data: {JsonConvert.SerializeObject(message)}");

                    var messageId = await messageService.InsertNewMessageAsync(message);
                    if (messageId > 0)
                    {
                        MessageEndpoint msgSendToEndpoint = new()
                        {
                            MessageId = messageId,
                            AppId = message.AppId,
                            ChannelId = (byte)message.Channel,
                            ConversationId = message.ConversationId,
                            Content = message.Content,
                        };
                        await messageQueueService.PublishAsync(msgSendToEndpoint);
                    }

                    logService.LogFileMessage($"End comsume {QueueSubcriptionId.INSERT_FROM_QUEUE_TO_DATABASE}");
                });
            }
        }

        private void SendMessageToEndpointConsumer()
        {
            var setting = GetConsumerSetting(QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT);
            if (setting is not null && setting.IsConsumed)
            {
                Subscribe<MessageEndpoint>(QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT,
                async (message) =>
                {
                    logService.LogFileMessage($"Begin consum: {QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT}. Data: {JsonConvert.SerializeObject(message)}");

                    var sentResult = await botClientService.SendMessageAsync((BotChannel)message.ChannelId, message);

                    await messageQueueService.PublishAsync(sentResult);

                    logService.LogFileMessage($"End comsume {QueueSubcriptionId.SEND_MESSAGE_TO_BOT_SERVICE_ENDPOINT}");
                });
            }
        }

        private void UpdateMessageStatusConsumer()
        {
            var setting = GetConsumerSetting(QueueSubcriptionId.UPDATE_MESSAGE_STATUS);
            if (setting is not null && setting.IsConsumed)
            {
                Subscribe<SendMessageResult>(QueueSubcriptionId.UPDATE_MESSAGE_STATUS,
                async (message) =>
                {
                    logService.LogFileMessage($"Begin consume: {QueueSubcriptionId.UPDATE_MESSAGE_STATUS}. Data: {JsonConvert.SerializeObject(message)}");

                    await messageService.UpdateMessageStatusAsync(message);

                    logService.LogFileMessage($"End comsume {QueueSubcriptionId.UPDATE_MESSAGE_STATUS}");
                });
            }
        }

        private ConsumerSetting? GetConsumerSetting(string queueName)
            => consumerSettings.FirstOrDefault(s => s.QueueName.Equals(queueName, StringComparison.OrdinalIgnoreCase));

        #region Subscribe

        private void Subscribe<T>(string subscriptionId, Action<T> action, int retryCount = 0)
        {
            bool isFailed = false;

            try
            {
                bus.PubSub.SubscribeAsync(
                   subscriptionId,
                   action);

                logService.LogFileMessage($"Subcribed the queue with key {subscriptionId} successfully");
            }
            catch (Exception ex)
            {
                logService.SentryErrorAsync($"There is an error while subscribing the queue with key {subscriptionId}. Retrying...", ex);
                isFailed = true;
            }

            if (isFailed && retryCount < MaxRetryTime)
            {
                Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetryInSeccond));
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