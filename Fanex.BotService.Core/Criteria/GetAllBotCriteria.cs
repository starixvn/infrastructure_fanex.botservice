namespace Fanex.BotService.Core.Criteria;

using Data.Repository;

public class GetAllBotCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Bot_GetAll";

    public override bool IsValid() => true;
}