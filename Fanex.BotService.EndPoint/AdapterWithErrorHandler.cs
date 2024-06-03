using System;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

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

                // Send a message to the user
                //await logService.SentryErrorAsync(JsonConvert.SerializeObject(turnContext.Activity), exception);

                if (conversationState != null)
                {
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
                }
                throw exception;
                //var errorMessageText = "The bot encountered an error or bug.  To continue to run this bot, please fix the bot source code.";
                //var errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.IgnoringInput);
                //await turnContext.SendActivityAsync(errorMessage);
            };
        }
    }
}