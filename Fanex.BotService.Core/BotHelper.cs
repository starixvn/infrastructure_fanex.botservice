namespace Fanex.NCB.BotService.Core
{
    public static class BotHelper
    {
        public static string GetMessageInGroupByChannel(string sourceMessage, string botName)
        {
            var returnMessage = sourceMessage;

            if (sourceMessage.StartsWith("@" + botName) || sourceMessage.StartsWith(botName))
            {
                var indexOfCommand = sourceMessage.IndexOf(' ');

                if (indexOfCommand > 0)
                {
                    returnMessage = sourceMessage.Remove(0, indexOfCommand).Trim();
                }
            }

            return returnMessage;
        }
    }
}
