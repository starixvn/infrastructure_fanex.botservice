namespace Fanex.BotService.EndPoint.Models
{
    public class JobSettings
    {
        public string JobName { get; set; }
        public bool IsEnabled { get; set; }
        public bool HasCheckUM { get; set; }
        public int TimeInterval { get; set; }
        public int CacheLiveTimeInMinute { get; set; } = 5;
    }
}