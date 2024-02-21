namespace Fanex.BotService.Api.Criteria
{
    using Fanex.Data.Repository;

    public class GetAppByIdCriteria : CriteriaBase
    {
        public int AppId { get; set; }

        public override string GetSettingKey() => "ApplicationSetting_GetById";

        public override bool IsValid() => AppId > 0;
    }
}