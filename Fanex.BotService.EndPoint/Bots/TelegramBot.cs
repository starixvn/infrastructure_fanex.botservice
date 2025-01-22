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

    public class TelegramBot : ActivityHandler, ITelegramBot
    {
        private const string greetingMessage = "Hello, I am {0}";

        private readonly IConversationService conversationService;

        public TelegramBot(IConversationService conversationService)
        {
            this.conversationService = conversationService;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var botName = turnContext.Activity.Recipient.Name;
            var message = turnContext.Activity.Text?.Trim() ?? string.Empty;

            if (message.StartsWith("/start"))
            {
                await conversationService.AddBotContactAsync(turnContext);
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(greetingMessage, botName)), cancellationToken);
            }

            if (turnContext.Activity.Conversation.IsGroup == true)
            {
                message = BotHelper.GetMessageInGroupByChannel(message, turnContext.Activity.ChannelId, botName);
                var channelData = turnContext.Activity.GetChannelData<TelegramData>();
                if (channelData.message.new_chat_member?.is_bot is true)
                {
                    await conversationService.AddBotToGroupAsync(turnContext);
                }
                else if (channelData.message.left_chat_member?.is_bot is true)
                {
                    await conversationService.RemoveBotOutGroupAsync(turnContext);
                }
            }

            switch (message)
            {
                case InteractedCommand.help:
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(greetingMessage, botName)), cancellationToken);
                    break;

                case InteractedCommand.getid:
                    await turnContext.SendActivityAsync(MessageFactory.Text(turnContext.Activity.Conversation.Id), cancellationToken);
                    break;

                case InteractedCommand.getbot:
                    var botInfo = $"BotId: {turnContext.Activity.Recipient.Id}, BotName: {botName}";
                    await turnContext.SendActivityAsync(MessageFactory.Text(botInfo), cancellationToken);
                    break;

                default: break;
            }
        }
    }
}