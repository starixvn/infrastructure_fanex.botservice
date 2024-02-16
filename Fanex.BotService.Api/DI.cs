namespace Fanex.BotService.Api
{
    using EasyNetQ;
    using Fanex.BotService.Service.Core.Configurations;
    using Fanex.BotService.Service.Core.Services;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Services;
    using Fanex.BotService.Shared.Services.Interfaces;

    public static class DI
    {
        public static void RegistryConfigurations(IConfiguration configuration)
        {
            AppSettings.RabbitMQConnectionString = configuration.GetValue<string>("RabbitMQConnectionString");
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddSingleton(RabbitHutch.CreateBus(AppSettings.RabbitMQConnectionString));

            services.AddSingleton<Logging.ILogger, Logging.Logger>();
            services.AddSingleton<ICommonLogService, CommonLogService>();
            services.AddSingleton<IMessageQueueService, MessageQueueService>();
        }
    }
}