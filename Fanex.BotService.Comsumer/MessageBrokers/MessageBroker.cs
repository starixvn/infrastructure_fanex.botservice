namespace Fanex.BotService.Comsumer.MessageBrokers
{
    using EasyNetQ;
    using Fanex.BotService.Comsumer.Services.Interfaces;
    using Fanex.BotService.Service.Core.Constants;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public interface IMessageBroker : IDisposable
    {
        void Start();
    }

    public class MessageBroker : IMessageBroker
    {
        private readonly IBus bus;
        private readonly ICommonLogService logService;
        private readonly IMessageReceiverService messageReceiverService;
        private readonly int TimeDelayBeforeRetryInSeccond = 10;
        private readonly int LimitRetryTime = 5;

        public MessageBroker(IBus bus,
             ICommonLogService logService,
             IMessageReceiverService messageReceiverService)
        {
            this.bus = bus;
            this.logService = logService;
            this.messageReceiverService = messageReceiverService;
        }

        public void Start()
        {
            ConversationConsume();
        }

        private void ConversationConsume()
        {
            Subscribe<Conversation>(QueueSubcriptionId.CLIENT_CONVERSATION_IN_QUEUE,
                async (message) =>
                {
                    logService.LogFileMessage($"Start Consume subcribtionId: {QueueSubcriptionId.CLIENT_CONVERSATION_IN_QUEUE}. Data: {JsonConvert.SerializeObject(message)}");

                    await messageReceiverService.ConversationConsumerHandlerAsync(message);

                    logService.LogFileMessage($"End comsume {QueueSubcriptionId.CLIENT_CONVERSATION_IN_QUEUE}");
                });
        }

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

            if (isFailed && retryCount < LimitRetryTime)
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