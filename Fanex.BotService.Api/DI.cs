namespace Fanex.BotService.Api
{
    using EasyNetQ;
    using Service.Core.Services;
    using Service.Core.Services.Interfaces;
    using Shared.Services;
    using Shared.Services.Interfaces;

    public static class DI
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            string rabbitMQConnectionString = configuration.GetValue<string>("RabbitMQConnectionString");
            services.AddHttpClient();
            services.AddSingleton(RabbitHutch.CreateBus(rabbitMQConnectionString));

            services.AddSingleton<Logging.ILogger, Logging.Logger>();
            services.AddSingleton<ICommonLogService, CommonLogService>();
            services.AddSingleton<IMessageQueueService, MessageQueueService>();
        }
    }
}