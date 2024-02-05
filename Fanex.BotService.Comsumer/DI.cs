namespace Fanex.BotService.Comsumer
{
    using EasyNetQ;
    using Fanex.BotService.Comsumer.MessageBrokers;
    using Fanex.BotService.Comsumer.Services;
    using Fanex.BotService.Comsumer.Services.Interfaces;
    using Fanex.BotService.Service.Core.Configurations;
    using Fanex.BotService.Service.Core.Services;
    using Fanex.BotService.Service.Core.Services.Interfaces;

    public static class DI
    {
        public static void StartMessageBroker(this IServiceCollection services)
        {
            var messageBroker = services.BuildServiceProvider().GetRequiredService<IMessageBroker>();
            messageBroker.Start();
        }

        public static void RegisterConfiguration(IConfiguration configuration)
        {
            AppSettings.RabbitMQConnectionString = configuration.GetValue<string>("RabbitMQConnectionString");
            AppSettings.BotEndpointConfiguration = configuration.GetSection("BotEndpointConfigurations").Get<BotEndpointConfiguration>();
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton(RabbitHutch.CreateBus(AppSettings.RabbitMQConnectionString));

            services.AddSingleton<IBotClientService, BotClientService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMessageReceiverService, MessageReceiverService>();

            services.AddSingleton<IMessageBroker, MessageBroker>();
        }
    }
}