namespace Fanex.BotService.EndPoint.BackgroundJobs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.BotService.EndPoint.Models;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Microsoft.Extensions.Hosting;

    public abstract class BaseWorker : BackgroundService
    {
        private readonly ICommonLogService logService;

        protected JobSettings jobSetting;

        protected BaseWorker(
            ICommonLogService logService)
        {
            this.logService = logService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const byte MillisecondsDelay = 10;
            await Task.Delay(MillisecondsDelay).ConfigureAwait(false);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await RunJobAsync();

                    await Task.Delay(TimeSpan.FromMinutes(jobSetting.TimeInterval), stoppingToken);
                }

                logService.LogFileMessage($"Execution ended. Cancelation token cancelled = {stoppingToken.IsCancellationRequested}");
            }
            catch (Exception ex)
            {
                logService.LogFileMessage("Unhandeled exception. Execution Stopping");
                logService.LogFileMessage(ex.Message);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logService.LogFileMessage($"{jobSetting.JobName}: Service Starting");
            return base.StartAsync(cancellationToken);
        }

        private async Task RunJobAsync()
        {
            try
            {
                if (jobSetting.IsEnabled
                    && (!jobSetting.HasCheckUM || !IsUM()))
                {
                    logService.LogFileMessage($"{jobSetting.JobName}: Doing...");
                    await ExecuteJobAsync();
                    logService.LogFileMessage($"{jobSetting.JobName}: Done.");
                }
            }
            catch (Exception ex)
            {
                _ = logService.SentryErrorAsync(ex.Message, ex);
                logService.LogFileMessage($"{jobSetting.JobName}: Error - {ex}");
            }
        }

        public abstract Task ExecuteJobAsync();

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logService.LogFileMessage($"{jobSetting.JobName}: Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            logService.LogFileMessage($"{jobSetting.JobName}: Disposing Service");
            GC.SuppressFinalize(this);
            base.Dispose();
        }

        private static bool IsUM()
            => false;
    }
}