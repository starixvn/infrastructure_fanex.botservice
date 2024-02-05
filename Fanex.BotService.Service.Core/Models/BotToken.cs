namespace Fanex.BotService.Service.Core.Models
{
    public class BotToken
    {
        public DateTimeOffset ExpiresIn { get; set; }
        public string AccessToken { get; set; }
    }
}