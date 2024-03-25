namespace Fanex.BotService.Client.Core
{
    using System;
    using Fanex.BotService.Client.Core.Configuration;

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