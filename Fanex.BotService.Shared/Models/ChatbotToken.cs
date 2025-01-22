namespace Fanex.BotService.Shared.Models
{
    public class ChatbotToken
    {
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string AccessToken { get; set; }
    }
}