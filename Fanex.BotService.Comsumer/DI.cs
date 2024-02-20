namespace Fanex.BotService.Comsumer
{
    using System.Net;
    using System.Net.Security;
    using System.Security.Authentication;
    using EasyNetQ;
    using Fanex.BotService.Comsumer.MessageBrokers;
    using Fanex.BotService.Comsumer.Services;
    using Fanex.BotService.Comsumer.Services.Interfaces;
    using Fanex.BotService.Service.Core.Configurations;
    using Fanex.BotService.Service.Core.Services;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Constants;
    using Fanex.BotService.Shared.Models;
    using Fanex.BotService.Shared.Services;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

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

            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IMemoryCache, MemoryCache>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<Logging.ILogger, Logging.Logger>();
            services.AddSingleton<ICommonLogService, CommonLogService>();

            services.AddSingleton<IBotClientService, BotClientService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IMessageReceiverService, MessageReceiverService>();

            services.AddSingleton<IMessageBroker, MessageBroker>();
        }

        public static void RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            var httpClientSettings = configuration.GetSection("HttpClientSetting").Get<HttpClientSettings>();

            if (httpClientSettings is not null)
            {
                services.AddHttpClient(NamedHttpClients.ProxiedClient).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
                {
                    UseProxy = httpClientSettings.IsUseProxy,
                    Proxy = new WebProxy(httpClientSettings.ProxyAddress, httpClientSettings.BypassOnLocal),
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        EnabledSslProtocols = SslProtocols.Tls12
                    }
                });
            }
        }
    }
}