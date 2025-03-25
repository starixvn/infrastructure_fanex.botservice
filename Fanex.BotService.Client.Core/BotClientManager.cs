namespace Fanex.BotService.Client.Core
{
    using Configuration;
    using System;

    public class BotClientManager
    {
        private IBotSettings _botSettings;
        private static readonly BotClientManager CurrentInstance = new BotClientManager();

        public static BotClientManager RegistryBot(IBotSettings settings)
        {
            CurrentInstance._botSettings = settings ?? throw new ArgumentNullException(nameof(settings));

            return CurrentInstance;
        }

        public static IBotSettings BotSettings
            => CurrentInstance._botSettings ?? throw new InvalidOperationException(nameof(CurrentInstance._botSettings));
    }
}