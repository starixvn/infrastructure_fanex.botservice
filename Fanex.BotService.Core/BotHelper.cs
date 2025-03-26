namespace Fanex.BotService.Core
{
    public static class BotHelper
    {
        public static string GetMessageInGroupByChannel(string sourceMessage, string channelId, string botName)
        {
            var returnMessage = sourceMessage;

            switch (channelId)
            {
                case "skype":
                    if (sourceMessage.StartsWith(botName))
                    {
                        returnMessage = sourceMessage.Remove(0, botName.Length).Trim();
                    }
                    break;

                case "telegram":
                    if (sourceMessage.StartsWith('@'))
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
                        var indexOfCommand = sourceMessage.IndexOf("</at>", System.StringComparison.Ordinal);

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

        public static string ReplaceSpecialCharacterOnTelegramMessageContent(string messageContent)
            => messageContent.Replace("[", "(").Replace("]", ")").Replace("\\", string.Empty);
    }
}