namespace Fanex.NCB.BotService.EndPoint
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Fanex.BotService.EndPoint;
    using Fanex.BotService.EndPoint.Bots.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotServiceDI
    {
        public static void Init(IServiceCollection services)
        {
            services.AddTransient<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
            services.AddTransient<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddTransient<ServiceClientCredentialsFactory, MultiServiceClientCredentialsFactory>();
            services.AddTransient<HttpClient, CustomHttpClient>();

            services.AddTransient<ITeamsBot, TeamsBot>();
            services.AddTransient<ISkypeBot, SkypeBot>();
            services.AddTransient<ITelegramBot, TelegramBot>();
            services.AddTransient<ICoreBot, CoreBot>();
            services.AddTransient<IBot, BotFactory>();

            services.AddScoped<Func<string, IChannelBot>>(serviceProvider => (channelType) =>
            {
                switch (channelType)
                {
                    case "msteams":
                        return serviceProvider.GetService<ITeamsBot>();

                    case "skype":
                        return serviceProvider.GetService<ISkypeBot>();

                    case "telegram":
                        return serviceProvider.GetService<ITelegramBot>();

                    default:
                        return serviceProvider.GetService<ICoreBot>();
                }
            });
        }
    }

    public class CustomHttpClient : HttpClient
    {
        public CustomHttpClient(IConfiguration configuration)
        {
            var isUseProxy = bool.Parse(configuration.GetSection("ProxySettings:IsUseProxy").Value);
            var proxyAddress = configuration.GetSection("ProxySettings:ProxyAddress").Value;
            var bypassOnLocal = bool.Parse(configuration.GetSection("ProxySettings:BypassOnLocal").Value);

            if (isUseProxy)
            {
                var proxy = new WebProxy(proxyAddress, bypassOnLocal);
                DefaultProxy = proxy;
            }
        }
    }
}