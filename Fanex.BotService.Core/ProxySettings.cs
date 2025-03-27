namespace Fanex.BotService.Core;

public class ProxySettings
{
    public bool IsUseProxy { get; set; }
    public string ProxyAddress { get; set; }
    public bool BypassOnLocal { get; set; }
    public int TimeOut { get; set; }
}