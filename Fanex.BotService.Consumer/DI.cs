namespace Fanex.BotService.Consumer;

using Data.Repository;
using EasyNetQ;
using MessageBrokers;
using Microsoft.Extensions.Caching.Memory;
using Service.Core.Configurations;
using Service.Core.Services;
using Service.Core.Services.Interfaces;
using Shared.Constants;
using Shared.Models;
using Shared.Repositories;
using Shared.Services;
using Shared.Services.Interfaces;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;

public static class Di
{
    public static void StartMessageBroker(this IServiceCollection services)
    {
        var messageBroker = services.BuildServiceProvider().GetRequiredService<IMessageBroker>();
        messageBroker.Start();
    }

    public static void RegisterConfiguration(IConfiguration configuration)
    {
        AppSettings.RabbitMqConnectionString = configuration.GetValue<string>("RabbitMQConnectionString");
        AppSettings.BotEndpointConfiguration = configuration.GetSection("BotEndpointConfigurations").Get<BotEndpointConfiguration>();
        AppSettings.ConsumerSettings = configuration.GetSection("ConsumerSettings").Get<List<ConsumerSetting>>();
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton(RabbitHutch.CreateBus(AppSettings.RabbitMqConnectionString));

        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddSingleton<IBaseRepository, BaseRepository>();
        services.AddSingleton<IMemoryCache, MemoryCache>();

        services.AddSingleton<IBaseService, BaseService>();
        services.AddSingleton<Logging.ILogger, Logging.Logger>();
        services.AddSingleton<ICommonLogService, CommonLogService>();

        services.AddSingleton<IBotClientService, BotClientService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IMessageQueueService, MessageQueueService>();

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