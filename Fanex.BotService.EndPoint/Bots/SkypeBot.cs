namespace Fanex.BotService.EndPoint.Bots
{
    using Core;
    using Fanex.BotService.Core.Services.Interfaces;
    using Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Shared.Services.Interfaces;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SkypeBot(IConversationService conversationService, ICommonLogService logService) : ActivityHandler, ISkypeBot
    {
        private const string GreetingMessage = "Hello, I am {0}";

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.MembersAdded != null)
            {
                if (turnContext.Activity.MembersAdded.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                {
                    await conversationService.AddBotToGroupAsync(turnContext);
                }
            }
            else
            {
                if (turnContext.Activity.MembersRemoved != null
                && turnContext.Activity.MembersRemoved.Any(m => m.Id == turnContext.Activity.Recipient?.Id))
                {
                    await conversationService.RemoveBotOutGroupAsync(turnContext);
                }
            }

            if (!string.IsNullOrWhiteSpace(turnContext.Activity.TopicName))
            {
                await conversationService.UpdateGroupTopicAsync(turnContext);
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var message = turnContext.Activity.Text.Trim();
            var botName = turnContext.Activity.Recipient.Name;
            logService.LogFileMessage($"OnMessageActivityAsync Telegram - message = {message} from {turnContext.Activity.From.Name}" +
                                      $" Id = {turnContext.Activity.From.Id}");

            if (turnContext.Activity.Conversation.IsGroup == true)
            {
                message = BotHelper.GetMessageInGroupByChannel(message, turnContext.Activity.ChannelId, botName);
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
            }
        }

        protected override async Task OnUnrecognizedActivityTypeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var botName = turnContext.Activity.Recipient.Name;
            if (turnContext.Activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                if (turnContext.Activity.Action == "add")
                {
                    await conversationService.AddBotContactAsync(turnContext);
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(GreetingMessage, botName)), cancellationToken);
                }
                else if (turnContext.Activity.Action == "remove")
                {
                    await conversationService.RemoveBotContactAsync(turnContext);
                    await turnContext.SendActivityAsync(MessageFactory.Text("see you"), cancellationToken);
                }
            }
        }
    }
}