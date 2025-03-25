namespace Fanex.BotService.Shared.Services
{
    using Interfaces;
    using NLog;
    using NLog.Web;
    using System;
    using System.Threading.Tasks;

    public class CommonLogService(Logging.ILogger logger) : ICommonLogService
    {
        private static readonly Logger NLog = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public void LogFileError(Exception exception)
        {
            NLog.Error(exception);
        }

        public void LogFileMessage(string message)
        {
            NLog.Error(message);
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