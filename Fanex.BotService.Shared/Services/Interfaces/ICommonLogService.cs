namespace Fanex.BotService.Shared.Services.Interfaces;

using System;
using System.Threading.Tasks;

public interface ICommonLogService
{
    Task SentryErrorAsync(string errMsg, Exception exception = null);

    Task SentryInfoAsync(string info);

    void LogFileMessage(string message);

    void LogFileError(Exception exception);
}