namespace Fanex.BotService.Shared.Extensions
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Authentication;
    using Fanex.BotService.Shared.Constants;
    using Fanex.BotService.Shared.Models;
    using Fanex.BotService.Shared.Services;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.Caching.Redis;
    using Fanex.Data.Repository;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Sentry;
    using StackExchange.Redis;

    public static class CoreServiceCollectionExtensions
    {
        public static void RegisterRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataProtection()
                .SetApplicationName(configuration.GetSection("Fanex.Caching.Redis:ApplicationName").Value)
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(configuration.GetSection("Fanex.Caching.Redis:ConnectionString").Value), "Fanex.BotService.API-Keys");

            services.AddRedisCache(configuration.GetSection("Fanex.Caching.Redis"));
        }

        public static void RegisterSentry(this IServiceCollection services, IConfiguration configuration)
        {
            LogManager
            .SetDefaultLogCategory(configuration.GetSection("Sentry:LogCategory").Value)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(configuration.GetSection("Sentry:Dsn").Value),
                })
            });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);
        }

        public static void RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            var httpClientSettings = configuration.GetSection("HttpClientSetting").Get<HttpClientSettings>();
            services.AddHttpClient();
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

        public static void RegisterCoreService(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseRepository, BaseRepository>();

            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ICommonLogService, CommonLogService>();
        }
    }
}