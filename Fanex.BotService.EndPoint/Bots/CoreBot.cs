namespace Fanex.NCB.BotService.EndPoint.Bots
{
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.BotService.EndPoint.Bots.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public class CoreBot : ActivityHandler, ICoreBot
    {
        private const string greetingMessage = "Hello, I am {0}";

        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(greetingMessage, turnContext.Activity.Recipient.Name)), cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(greetingMessage, turnContext.Activity.Recipient.Name)), cancellationToken);
        }
    }
}