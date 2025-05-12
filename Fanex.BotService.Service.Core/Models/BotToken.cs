namespace Fanex.BotService.Service.Core.Models;

public class BotToken
{
    public DateTime? Expires { get; set; }
    public string AccessToken { get; set; }
}