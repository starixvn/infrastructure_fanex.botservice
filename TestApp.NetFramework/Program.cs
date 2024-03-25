using System;
using System.Threading.Tasks;
using Fanex.BotService.Client.AspNetMvc;
using Fanex.BotService.Client.Core;
using Fanex.BotService.Client.Core.Services;

namespace TestApp.NetFramework
{
    internal class Program
    {
        public static async Task Main()
        {
            BotClientManager.RegistryBot(new BotSettings());
            var bot = new BotService();
            try
            {
                await bot.SendSkypeAsync("29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ", "Hi Oliver, this is the message Test Skype");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
            Console.WriteLine();
        }
    }
}