namespace Fanex.BotService.Service.Core.Configurations
{
    using Fanex.BotService.Service.Core.Enum;

    public static class AppSettings
    {
        public static string RabbitMQConnectionString { get; set; }
        public static BotEndpointConfiguration BotEndpointConfiguration { get; set; }
    }

    public class BotEndpointConfiguration
    {
        public string ServiceUrl { get; set; }
        public string AuthenticatePath { get; set; }
        public string SendMessagePath { get; set; }
        public string AppId { get; set; }
        public string AppPassword { get; set; }
        public List<ApiConfig> ApiConfigs { get; set; }
    }

    public class ApiConfig
    {
        public BotChannel Channel { get; set; }
        public string SendMessageUrl { get; set; }
    }
}