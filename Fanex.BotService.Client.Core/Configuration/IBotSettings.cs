namespace Fanex.BotService.Client.Core.Configuration
{
    public interface IBotSettings
    {
        int AppId { get; }
        string ServiceUrl { get; }
        string SecretKey { get; }
    }
}