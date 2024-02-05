namespace Fanex.BotService.Client.AspNetCore
{
    using System.ComponentModel;
    using Fanex.BotService.Client.Core.Configuration;
    using Microsoft.Extensions.Configuration;

    public class BotSettings : IBotSettings
    {
        private readonly IConfiguration configuration;

        public BotSettings(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            AppId = GetValue<int>("Fanex.BotService:AppId");
            SecretKey = GetValue<string>("Fanex.BotService:SecretKey") ?? throw new ArgumentNullException(SecretKey); ;
            ServiceUrl = GetValue<string>("Fanex.BotService:ServiceUrl") ?? throw new ArgumentNullException(ServiceUrl); ;
        }

        public int AppId { get; set; }

        public string SecretKey { get; set; }

        public string ServiceUrl { get; set; }

        private T GetValue<T>(string key)
        {
            var value = configuration.GetSection(key).Value;
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return (T)typeConverter.ConvertFromString(value);
        }
    }
}