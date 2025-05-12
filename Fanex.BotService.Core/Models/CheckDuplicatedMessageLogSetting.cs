namespace Fanex.BotService.Core.Models;

public class CheckDuplicatedMessageLogSetting
{
    public bool IsEnabled { get; set; }

    public int TimeSpanInMinutes { get; set; }

    public string[] ExceptionConversatiionIDs { get; set; }
}