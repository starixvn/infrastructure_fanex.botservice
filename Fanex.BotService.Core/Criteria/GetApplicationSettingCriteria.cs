namespace Fanex.BotService.Core.Criteria
{
    using Data.Repository;

    public class GetApplicationSettingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "ApplicationSetting_Get";

        public override bool IsValid() => true;
    }
}