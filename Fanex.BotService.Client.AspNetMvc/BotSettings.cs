namespace Fanex.BotService.Client.AspNetMvc
{
    using System.ComponentModel;
    using System.Configuration;
    using Fanex.BotService.Client.Core.Configuration;

    public class BotSettings : IBotSettings
    {
        public BotSettings()
        {
            AppId = GetValue<int>("Fanex.BotService:AppId");
            ServiceUrl = GetValue<string>("Fanex.BotService:ServiceUrl");
        }

        public BotSettings(int appId, string serviceUrl)
        {
            AppId = appId;
            ServiceUrl = serviceUrl;
        }

        public int AppId { get; set; }

        public string ServiceUrl { get; set; }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}