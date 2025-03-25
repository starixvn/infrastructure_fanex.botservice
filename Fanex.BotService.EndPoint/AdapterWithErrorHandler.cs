using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace Fanex.BotService.EndPoint
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        public AdapterWithErrorHandler(BotFrameworkAuthentication auth, ILogger<IBotFrameworkHttpAdapter> logger, ICommonLogService logService, ConversationState conversationState = null)
            : base(auth, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                await logService.SentryErrorAsync(JsonConvert.SerializeObject(turnContext.Activity), exception);

                if (conversationState == null)
                {
                    throw exception;
                }

                try
                {
                    await conversationState.DeleteAsync(turnContext);
                }
                catch (Exception e)
                {
                    await logService.SentryErrorAsync($"Exception caught on attempting to Delete ConversationState : {e.Message}", e);
                }
                throw exception;
            };
        }
    }
}