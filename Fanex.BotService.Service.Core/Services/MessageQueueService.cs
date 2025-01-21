namespace Fanex.BotService.Service.Core.Services
{
    using EasyNetQ;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Interfaces;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    public class MessageQueueService(IBus bus, ICommonLogService logService) : IMessageQueueService
    {
        private const int TimeDelayBeforeRetry = 10;
        private const int MaxRetryTime = 5;

        public async Task PublishAsync<T>(T model, int retryCount = 0)
        {
            while (true)
            {
                var isFailed = false;
                try
                {
                    await bus.PubSub.PublishAsync(model);
                    logService.LogFileMessage($"Published model {JsonConvert.SerializeObject(model)} successfully!");
                }
                catch (Exception ex)
                {
                    isFailed = true;
                    await logService.SentryErrorAsync($"There is an error while publish object: {JsonConvert.SerializeObject(model)}. Retrying {retryCount + 1} time", ex);
                }

                if (isFailed && retryCount < MaxRetryTime)
                {
                    retryCount++;
                    await Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetry));
                    continue;
                }

                break;
            }
        }

        public async Task PublishWithTopicAsync<T>(T model, string topic, int retryCount = 0)
        {
            while (true)
            {
                var isFailed = false;
                try
                {
                    await bus.PubSub.PublishAsync(model, topic);
                    logService.LogFileMessage($"Published model {JsonConvert.SerializeObject(model)} with topic {topic} successfully!");
                }
                catch (Exception ex)
                {
                    isFailed = true;
                    await logService.SentryErrorAsync($"There is an error while publish object: {JsonConvert.SerializeObject(model)}. Topic: {topic} Retrying {retryCount + 1} time", ex);
                }

                if (isFailed && retryCount < MaxRetryTime)
                {
                    retryCount++;
                    await Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetry));
                    continue;
                }

                break;
            }
        }

        public async Task SchedulerPublishAsync<T>(T model, int interval, int retryCount = 0)
        {
            while (true)
            {
                var isFailed = false;
                try
                {
                    await bus.Scheduler.FuturePublishAsync(model, TimeSpan.FromMinutes(interval));
                    logService.LogFileMessage($"Future publish after {interval} minutes. Data: {JsonConvert.SerializeObject(model)} successfully!");
                }
                catch (Exception ex)
                {
                    isFailed = true;
                    await logService.SentryErrorAsync($"There is an error while scheduler publish object: {JsonConvert.SerializeObject(model)}. Retrying {retryCount + 1} time", ex);
                }

                if (isFailed && retryCount < MaxRetryTime)
                {
                    retryCount++;
                    await Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetry));
                    continue;
                }

                break;
            }
        }
    }
}