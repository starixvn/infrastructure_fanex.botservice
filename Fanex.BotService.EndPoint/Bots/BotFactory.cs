namespace Fanex.BotService.EndPoint.Bots
{
    using Interfaces;
    using Microsoft.Bot.Builder;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class BotFactory : IBot
    {
        private readonly Func<string, IChannelBot> botFactory;

        public BotFactory(Func<string, IChannelBot> botFactory)
        {
            this.botFactory = botFactory;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await botFactory(turnContext.Activity.ChannelId).OnTurnAsync(turnContext, cancellationToken);
        }
    }
}