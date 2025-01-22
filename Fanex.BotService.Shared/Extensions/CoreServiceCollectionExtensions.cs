namespace Fanex.BotService.Shared.Extensions
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Logging;
    using Microsoft.Extensions.Configuration;
    using Sentry;
    using System.Collections.Generic;

    public static class CoreServiceCollectionExtensions
    {
        public static void RegisterSentry(IConfiguration configuration)
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