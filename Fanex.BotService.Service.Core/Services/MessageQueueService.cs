namespace Fanex.BotService.Service.Core.Services
{
    using System.Threading.Tasks;
    using EasyNetQ;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class MessageQueueService : IMessageQueueService
    {
        private readonly IBus bus;
        private readonly ICommonLogService logService;
        private readonly int TimeDelayBeforeRetryInSeccond = 10;
        private readonly int MaxRetryTime = 5;

        public MessageQueueService(IBus bus, ICommonLogService logService)
        {
            this.bus = bus;
            this.logService = logService;
        }

        public async Task PublishAsync<T>(T model, int retryCount = 0)
        {
            bool isFailed = false;
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
                await Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetryInSeccond));
                await PublishAsync(model, retryCount);
            }
        }

        public async Task PublishAsync<T>(T model, string topic, int retryCount = 0)
        {
            bool isFailed = false;
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
                await Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetryInSeccond));
                await PublishAsync(model, topic, retryCount);
            }
        }

        public async Task SchedulerPublishAsync<T>(T model, int interval, int retryCount = 0)
        {
            bool isFailed = false;
            try
            {
                await bus.Scheduler.FuturePublishAsync(model, TimeSpan.FromMinutes(interval));
                logService.LogFileMessage($"Future publish after {interval} mins. Data: {JsonConvert.SerializeObject(model)} successfully!");
            }
            catch (Exception ex)
            {
                isFailed = true;
                await logService.SentryErrorAsync($"There is an error while scheduler publish object: {JsonConvert.SerializeObject(model)}. Retrying {retryCount + 1} time", ex);
            }

            if (isFailed && retryCount < MaxRetryTime)
            {
                retryCount++;
                await Task.Delay(TimeSpan.FromSeconds(TimeDelayBeforeRetryInSeccond));
                await SchedulerPublishAsync(model, interval, retryCount);
            }
        }
    }
}