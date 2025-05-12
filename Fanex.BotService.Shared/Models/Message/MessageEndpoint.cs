namespace Fanex.BotService.Shared.Models.Message;

using System.Collections.Generic;

public class MessageEndpoint
{
    public int MessageId { get; set; }
    public int AppId { get; set; }
    public string ConversationId { get; set; }
    public string Content { get; set; }
    public byte ChannelId { get; set; }
    public List<string> Images { get; set; }
}