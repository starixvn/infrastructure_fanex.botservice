using EasyNetQ;
using Fanex.BotService.Service.Core.Services;
using Fanex.BotService.Shared.Services.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Fanex.BotService.Core.Tests.Services;

public class MessageQueueServiceTests
{
    private readonly IBus _bus;
    private readonly ICommonLogService _logService;
    private readonly MessageQueueService _messageQueueService;

    public MessageQueueServiceTests()
    {
        _bus = Substitute.For<IBus>();
        _logService = Substitute.For<ICommonLogService>();
        _messageQueueService = new MessageQueueService(_bus, _logService);
    }

    [Fact]
    public async Task PublishAsync_ShouldLogSuccessMessage_WhenPublishSucceeds()
    {
        var model = new { Name = "Test" };

        await _messageQueueService.PublishAsync(model);

        await _bus.PubSub.Received(1).PublishAsync(model);
        _logService.Received(1).LogFileMessage(Arg.Is<string>(msg => msg.Contains("Published model")));
    }

    [Fact]
    public async Task PublishAsync_ShouldRetry_WhenPublishFails()
    {
        var model = new { Name = "Test" };
        _bus.PubSub.PublishAsync(model).Throws(new Exception("Publish failed"));

        await _messageQueueService.PublishAsync(model);

        await _bus.PubSub.Received(6).PublishAsync(model);
        await _logService.Received(6).SentryErrorAsync(Arg.Is<string>(msg => msg.Contains("Retrying")), Arg.Any<Exception>());
    }

    [Fact]
    public async Task PublishAsync_WithTopic_ShouldLogSuccessMessage_WhenPublishSucceeds()
    {
        var model = new { Name = "Test" };
        var topic = "test-topic";

        await _messageQueueService.PublishAsync(model, topic);

        await _bus.PubSub.Received(1).PublishAsync(model, topic);
        //_logService.Received(1).LogFileMessage(Arg.Is<string>(msg => msg.Contains("Published model") && msg.Contains(topic)));
    }

    [Fact]
    public async Task SchedulerPublishAsync_ShouldLogSuccessMessage_WhenFuturePublishSucceeds()
    {
        var model = new { Name = "Test" };
        var interval = 10;

        await _messageQueueService.SchedulerPublishAsync(model, interval);

        await _bus.Scheduler.Received(1).FuturePublishAsync(model, TimeSpan.FromMinutes(interval));
        _logService.Received(1).LogFileMessage(Arg.Is<string>(msg => msg.Contains("Future publish after")));
    }

    [Fact]
    public async Task SchedulerPublishAsync_ShouldRetry_WhenFuturePublishFails()
    {
        var model = new { Name = "Test" };
        var interval = 10;
        _bus.Scheduler.FuturePublishAsync(model, TimeSpan.FromMinutes(interval)).Throws(new Exception("Future publish failed"));

        await _messageQueueService.SchedulerPublishAsync(model, interval);

        await _bus.Scheduler.Received(6).FuturePublishAsync(model, TimeSpan.FromMinutes(interval));
        await _logService.Received(6).SentryErrorAsync(Arg.Is<string>(msg => msg.Contains("Retrying")), Arg.Any<Exception>());
    }
}