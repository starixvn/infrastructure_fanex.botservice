namespace Fanex.BotService.Client.AspNetCore
{
    using Fanex.BotService.Client.Core;
    using Fanex.BotService.Client.Core.Configuration;
    using Fanex.BotService.Client.Core.Services;
    using Fanex.BotService.Client.Core.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class FanexBotRegister
    {
        public static void RegisterBotClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBotSettings, BotSettings>();
            services.AddSingleton<IBotService, BotService>();

            BotClientManager.RegistryBot(new BotSettings(configuration));
        }
    }
}