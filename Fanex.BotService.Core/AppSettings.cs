namespace Fanex.NCB.BotService.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public class AppSettings
    {
        public string Domain { get; set; }
        public string AppCredentialId { get; set; }
        public string AppCredentialPassword { get; set; }
        public string Secret { get; set; }
        public string AllowedHosts { get; set; }
        public string EnableChannelIds { get; set; }
        public bool IsEnabledSendIndirect { get; set; }
        public bool IsOnlySendIndirect { get; set; }
        public bool IsCacheDataInRedis { get; set; }
        public string AuthenticateUrl { get; set; }
        public string FowardUrl { get; set; }
        public int JwtLifeTime { get; set; } = 120;
        public int BotListCacheLifeTime { get; set; } = 5;

        public IEnumerable<byte> EnableChannels => !string.IsNullOrWhiteSpace(EnableChannelIds)
            ? EnableChannelIds.Split(',').Select(x => byte.Parse(x)) : new List<byte>();
    }
}