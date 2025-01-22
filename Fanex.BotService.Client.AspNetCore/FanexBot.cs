namespace Fanex.BotService.Client.AspNetCore
{
    using Core;
    using Core.Configuration;
    using Core.Services;
    using Core.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class FanexBot
    {
        public static void RegisterBotClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBotSettings, BotSettings>();
            services.AddSingleton<IBotService, BotService>();
            BotClientManager.RegistryBot(new BotSettings(configuration));
        }
    }
}