namespace Fanex.BotService.Service.Core.Services.Interfaces;

public interface IMessageQueueService
{
    Task PublishAsync<T>(T model, int retryCount = 0);

    Task PublishWithTopicAsync<T>(T model, string topic, int retryCount = 0);

    Task SchedulerPublishAsync<T>(T model, int interval, int retryCount = 0);
}