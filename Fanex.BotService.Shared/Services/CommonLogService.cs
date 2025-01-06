namespace Fanex.BotService.Shared.Services
{
    using Fanex.BotService.Shared.Services.Interfaces;
    using NLog;
    using NLog.Web;
    using System;
    using System.Threading.Tasks;

    public class CommonLogService : ICommonLogService
    {
        private readonly Logging.ILogger logger;
        private readonly Logger nlog = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public CommonLogService(Logging.ILogger logger)
        {
            this.logger = logger;
        }

        public void LogFileError(Exception exception)
        {
            nlog.Error(exception);
        }

        public void LogFileMessage(string message)
        {
            nlog.Error(message);
        }

        public async Task SentryErrorAsync(string errMsg, Exception exception = null)
        {
            await logger.ErrorAsync(errMsg, exception);
        }

        public async Task SentryInfoAsync(string info)
        {
            await logger.InfoAsync(info);
        }
    }
}