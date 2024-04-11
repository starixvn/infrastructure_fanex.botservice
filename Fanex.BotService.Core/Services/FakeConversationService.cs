namespace Fanex.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.BotService.Core;
    using Fanex.BotService.Core.Criteria;
    using Fanex.BotService.Core.Models;
    using Fanex.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Logging;

    public class FakeConversationService : IConversationService
    {
        private readonly IList<BotConfigs> botConfigs;
        private readonly ILogger<MessageSenderService> filelogger;

        public FakeConversationService(IList<BotConfigs> botConfigs, ILogger<MessageSenderService> filelogger)
        {
            this.botConfigs = botConfigs;
            this.filelogger = filelogger;
        }

        public async Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync()
        {
            var applicationSettings = botConfigs.Select(t =>
                new ApplicationSetting
                {
                    AppId = t.AppId,
                    MicrosoftAppId = t.MicrosoftAppId,
                    MicrosoftAppPassword = t.MicrosoftAppPassword,
                    MicrosoftAppTenantId = t.MicrosoftAppTenantId
                });

            return await Task.FromResult(applicationSettings);
        }

        public async Task<IEnumerable<Bot>> GetBotAsync()
        {
            var bots = new List<Bot>();
            foreach (var botConfig in botConfigs)
            {
                foreach (var channel in botConfig.Channels)
                {
                    bots.Add(new Bot
                    {
                        AppId = botConfig.AppId,
                        MicrosoftAppId = botConfig.MicrosoftAppId,
                        ChannelId = channel.ChannelId,
                        BotName = channel.BotName,
                        ConversationId = channel.BotId,
                        ChannelName = channel.ChannelName,
                        ServiceUrl = channel.ServiceUrl
                    });
                }
            }

            return await Task.FromResult(bots);
        }

        public async Task<CommandResult> AddBotContactAsync(ITurnContext turnContext)
        {
            var microsoftAppId = ((AppCredentials)((ConnectorClient)turnContext.TurnState.ToList()[1].Value).Credentials).MicrosoftAppId;

            var command = new AddBotContactCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                BotName = turnContext.Activity?.Recipient.Name,
                ConversationId = turnContext.Activity?.Conversation.Id,
                ChannelName = turnContext.Activity?.ChannelId,
                DisplayName = turnContext.Activity?.From.Name,
                ServiceUrl = turnContext.Activity?.ServiceUrl,
                MicrosoftAppId = microsoftAppId
            };

            var commandJson = JsonSerializer.Serialize(command);
            filelogger.LogError(commandJson);
            return await Task.FromResult(new CommandResult());
        }

        public async Task<CommandResult> RemoveBotContactAsync(ITurnContext turnContext)
        {
            var command = new RemoveBotContactCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                ConversationId = turnContext.Activity?.Conversation.Id
            };
            var commandJson = JsonSerializer.Serialize(command);
            filelogger.LogError(commandJson);

            return await Task.FromResult(new CommandResult());
        }

        public async Task<CommandResult> AddBotToGroupAsync(ITurnContext turnContext)
        {
            var microsoftAppId = ((AppCredentials)((ConnectorClient)turnContext.TurnState.ToList()[1].Value).Credentials).MicrosoftAppId;

            var command = new AddBotToGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                BotName = turnContext.Activity?.Recipient.Name,
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = turnContext.Activity?.TopicName,
                ChannelName = turnContext.Activity?.ChannelId,
                ServiceUrl = turnContext.Activity?.ServiceUrl,
                UpdatedBy = turnContext.Activity?.From.Id,
                MicrosoftAppId = microsoftAppId
            };
            var commandJson = JsonSerializer.Serialize(command);
            filelogger.LogError(commandJson);

            return await Task.FromResult(new CommandResult());
        }

        public async Task<CommandResult> RemoveBotOutGroupAsync(ITurnContext turnContext)
        {
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                ConversationId = turnContext.Activity?.Conversation.Id,
                UpdatedBy = turnContext.Activity?.From.Id
            };
            var commandJson = JsonSerializer.Serialize(command);
            filelogger.LogError(commandJson);

            return await Task.FromResult(new CommandResult());
        }

        public async Task<CommandResult> UpdateGroupTopicAsync(ITurnContext turnContext)
        {
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = HttpUtility.HtmlDecode(turnContext.Activity?.TopicName),
                UpdatedBy = turnContext.Activity?.From.Id
            };
            var commandJson = JsonSerializer.Serialize(command);
            filelogger.LogError(commandJson);

            return await Task.FromResult(new CommandResult());
        }

        public async Task<CommandResult> MigratedToSupergroupAsync(MigratedToSupergroupCommand command)
        {
            return await Task.FromResult(new CommandResult());
        }

        public async Task<CommandResult> AddMessageLog(AddMessageLogCommand command)
        {
            var commandJson = JsonSerializer.Serialize(command);
            filelogger.LogError(commandJson);
            return await Task.FromResult(new CommandResult());
        }
    }
}