namespace Fanex.BotService.Core.Services.Interfaces;

using Criteria;
using Fanex.BotService.Shared.Models;
using Microsoft.Bot.Builder;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IConversationService
{
    Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync();

    Task<IEnumerable<Bot>> GetBotAsync();

    Task<CommandResult> AddBotContactAsync(ITurnContext turnContext);

    Task<CommandResult> RemoveBotContactAsync(ITurnContext turnContext);

    Task<CommandResult> AddBotToGroupAsync(ITurnContext turnContext);

    Task<CommandResult> RemoveBotOutGroupAsync(ITurnContext turnContext);

    Task<CommandResult> UpdateGroupTopicAsync(ITurnContext turnContext);

    Task<CommandResult> MigratedToSupergroupAsync(MigratedToSupergroupCommand command);

    Task<CommandResult> AddMessageLog(AddMessageLogCommand command);
}