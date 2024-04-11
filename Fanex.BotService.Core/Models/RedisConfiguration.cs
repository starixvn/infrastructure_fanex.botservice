namespace Fanex.BotService.Core.Models
{
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; }

        public string ApplicationName { get; set; }

        public int Db { get; set; }
    }
}