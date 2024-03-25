namespace Fanex.BotService.Shared.Extensions
{
    using System.Collections.Generic;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using Microsoft.Extensions.Configuration;
    using Sentry;

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