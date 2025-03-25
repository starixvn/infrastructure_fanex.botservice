namespace Fanex.BotService.EndPoint.Bots
{
    using Core;
    using Core.Models;
    using Fanex.BotService.Core.Services.Interfaces;
    using Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using System.Threading;
    using System.Threading.Tasks;

    public class TelegramBot(IConversationService conversationService) : ActivityHandler, ITelegramBot
    {
        private const string GreetingMessage = "Hello, I am {0}";

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var botName = turnContext.Activity.Recipient.Name;
            var message = turnContext.Activity.Text?.Trim() ?? string.Empty;

            if (message.StartsWith("/start"))
            {
                await conversationService.AddBotContactAsync(turnContext);
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(GreetingMessage, botName)), cancellationToken);
            }

            if (turnContext.Activity.Conversation.IsGroup == true)
            {
                message = BotHelper.GetMessageInGroupByChannel(message, turnContext.Activity.ChannelId, botName);
                var channelData = turnContext.Activity.GetChannelData<TelegramData>();
                if (channelData.Message.NewChatMember?.IsBot is true)
                {
                    await conversationService.AddBotToGroupAsync(turnContext);
                }
                else if (channelData.Message.LeftChatMember?.IsBot is true)
                {
                    await conversationService.RemoveBotOutGroupAsync(turnContext);
                }
            }

            switch (message)
            {
                case InteractedCommand.Help:
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(GreetingMessage, botName)), cancellationToken);
                    break;

                case InteractedCommand.GetId:
                    await turnContext.SendActivityAsync(MessageFactory.Text(turnContext.Activity.Conversation.Id), cancellationToken);
                    break;

                case InteractedCommand.GetBot:
                    var botInfo = $"BotId: {turnContext.Activity.Recipient.Id}, BotName: {botName}";
                    await turnContext.SendActivityAsync(MessageFactory.Text(botInfo), cancellationToken);
                    break;

                default: break;
            }
        }
    }
}