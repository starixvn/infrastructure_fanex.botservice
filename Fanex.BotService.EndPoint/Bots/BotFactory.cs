namespace Fanex.BotService.EndPoint.Bots;

using Interfaces;
using Microsoft.Bot.Builder;
using System;
using System.Threading;
using System.Threading.Tasks;

public class BotFactory(Func<string, IChannelBot> botFactory) : IBot
{
    public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
        await botFactory(turnContext.Activity.ChannelId).OnTurnAsync(turnContext, cancellationToken);
    }
}