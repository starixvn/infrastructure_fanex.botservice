namespace Fanex.BotService.Core.Models;

public class TelegramData
{
    public long UpdateId { get; set; }
    public Message Message { get; set; }
}

public class Message
{
    public long MessageId { get; set; }
    public From From { get; set; }
    public Chat Chat { get; set; }
    public Member NewChatMember { get; set; }
    public Member LeftChatMember { get; set; }
    public long Date { get; set; }
    public string Text { get; set; }
}

public class From
{
    public long Id { get; set; }
    public bool IsBot { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string LanguageCode { get; set; }
}

public class Chat
{
    public long Id { get; set; }
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