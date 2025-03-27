namespace Fanex.BotService.EndPoint.Bots;

using Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Shared.Services.Interfaces;
using System.Threading;
using System.Threading.Tasks;

public class CoreBot(ICommonLogService logService) : ActivityHandler, ICoreBot
{
    private const string GreetingMessage = "Hello, I am {0}";

    protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
    {
        logService.LogFileMessage($"OnConversationUpdateActivityAsync: {turnContext.Activity.TopicName} from {turnContext.Activity.From.Name}" +
                                  $" Id = {turnContext.Activity.From.Id}");
        await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(GreetingMessage, turnContext.Activity.Recipient.Name)), cancellationToken);
    }

    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
    {
        logService.LogFileMessage($"Received a message: {turnContext.Activity.Text} from {turnContext.Activity.From.Name}" +
                                  $" Id = {turnContext.Activity.From.Id}");
        await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(GreetingMessage, turnContext.Activity.Recipient.Name)), cancellationToken);
    }
}