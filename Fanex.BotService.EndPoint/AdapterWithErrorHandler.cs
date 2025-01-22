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
        public AdapterWithErrorHandler(BotFrameworkAuthentication auth, ILogger<IBotFrameworkHttpAdapter> logger, ICommonLogService logService, ConversationState conversationState = default)
            : base(auth, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                // NOTE: In production environment, you should consider logging this to
                // Azure Application Insights. Visit https://aka.ms/bottelemetry to see how
                // to add telemetry capture to your bot.

                await logService.SentryErrorAsync(JsonConvert.SerializeObject(turnContext.Activity), exception);

                if (conversationState == null)
                {
                    throw exception;
                }

                try
                {
                    // Delete the conversationState for the current conversation to prevent the
                    // bot from getting stuck in a error-loop caused by being in a bad state.
                    // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
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