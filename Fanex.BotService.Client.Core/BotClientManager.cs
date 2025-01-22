namespace Fanex.BotService.Client.Core
{
    using Configuration;
    using System;

    public class BotClientManager
    {
        private IBotSettings botSettings;
        private static readonly BotClientManager CurrentInstance = new BotClientManager();

        public static BotClientManager RegistryBot(IBotSettings settings)
        {
            CurrentInstance.botSettings = settings ?? throw new ArgumentNullException(nameof(settings));

            return CurrentInstance;
        }

        public static IBotSettings BotSettings
            => CurrentInstance.botSettings ?? throw new InvalidOperationException(nameof(CurrentInstance.botSettings));
    }
}