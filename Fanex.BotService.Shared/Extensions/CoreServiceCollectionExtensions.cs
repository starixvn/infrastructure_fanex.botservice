namespace Fanex.BotService.Shared.Extensions
{
    using System.Collections.Generic;
    using Fanex.Caching.Redis;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
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
    }
}