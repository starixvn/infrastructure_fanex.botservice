using Fanex.NCB.BotService.Core.Enum;

namespace Fanex.NCB.BotService.Core
{
    public static class BotHelper
    {
        public static string GetMessageInGroupByChannel(string sourceMessage, string channelId, string botName)
        {
            var returnMessage = sourceMessage;

            switch (channelId)
            {
                case "skype":
                    if (sourceMessage.StartsWith('@')
                        || sourceMessage.StartsWith(botName))
                    {
                        var indexOfCommand = sourceMessage.IndexOf(' ');

                        if (indexOfCommand > 0)
                        {
                            returnMessage = sourceMessage.Remove(0, indexOfCommand).Trim();
                        }
                    }
                    break;
                case "telegram":
                    if (sourceMessage.StartsWith('/'))
                    {
                        var indexOfCommand = sourceMessage.IndexOf(' ');

                        if (indexOfCommand > 0)
                        {
                            returnMessage = sourceMessage.Remove(0, indexOfCommand).Trim();
                        }
                    }
                    break;
                case "msteams":
                    if (sourceMessage.StartsWith("<at>"))
                    {
                        var indexOfCommand = sourceMessage.IndexOf("</at>");

                        if (indexOfCommand > 0)
                        {
                            returnMessage = sourceMessage.Remove(0, indexOfCommand + 5).Trim();
                        }
                    }
                    break;
                default:
                    returnMessage = sourceMessage;
                    break;
            }

            return returnMessage;
        }
    }
}
