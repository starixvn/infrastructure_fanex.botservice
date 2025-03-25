namespace Fanex.BotService.Api
{
    using EasyNetQ;
    using Service.Core.Services;
    using Service.Core.Services.Interfaces;
    using Shared.Services;
    using Shared.Services.Interfaces;

    public static class Di
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConnectionString = configuration.GetValue<string>("RabbitMQConnectionString");
            services.AddHttpClient();
            services.AddSingleton(RabbitHutch.CreateBus(rabbitMqConnectionString));

            services.AddSingleton<Logging.ILogger, Logging.Logger>();
            services.AddSingleton<ICommonLogService, CommonLogService>();
            services.AddSingleton<IMessageQueueService, MessageQueueService>();
        }
    }
}