namespace Fanex.BotService.Api
{
    using EasyNetQ;
    using Fanex.BotService.Service.Core.Configurations;
    using Fanex.BotService.Service.Core.Services;
    using Fanex.BotService.Service.Core.Services.Interfaces;

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

            services.AddSingleton<IMessageQueueService, MessageQueueService>();
        }
    }
}