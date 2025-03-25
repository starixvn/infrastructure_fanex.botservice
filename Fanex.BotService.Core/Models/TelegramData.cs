namespace Fanex.BotService.Core.Models
{
    public class TelegramData
    {
        public int UpdateId { get; set; }
        public Message Message { get; set; }
    }

    public class Message
    {
        public int MessageId { get; set; }
        public From From { get; set; }
        public Chat Chat { get; set; }

        public Member NewChatMember { get; set; }

        public Member LeftChatMember { get; set; }

        public int Date { get; set; }
        public string Text { get; set; }
    }

    public class From
    {
        public int Id { get; set; }
        public bool IsBot { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string LanguageCode { get; set; }
    }

    public class Chat
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Type { get; set; }
    }

    public class Member
    {
        public long Id { get; set; }
        public bool IsBot { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
    }
}
