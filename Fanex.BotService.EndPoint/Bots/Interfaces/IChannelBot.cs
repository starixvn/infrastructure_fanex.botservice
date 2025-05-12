namespace Fanex.BotService.EndPoint.Bots.Interfaces;

using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;

public interface IChannelBot
{
    Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default);
}