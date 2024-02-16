namespace Fanex.BotService.Shared.Models
{
    public class HttpClientSettings
    {
        public bool IsUseProxy { get; set; }
        public string ProxyAddress { get; set; }
        public bool BypassOnLocal { get; set; }
        public int TimeOut { get; set; } = 120;
    }
}