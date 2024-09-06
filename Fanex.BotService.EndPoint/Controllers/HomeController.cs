namespace Fanex.BotService.EndPoint.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Schema;

    public class HomeController(IBotFrameworkHttpAdapter adapter) : ControllerBase
    {
        public IActionResult Index()
        {
            return Content("Fanex bot is ready!");
        }

        public async Task<IActionResult> TestBot()
        {
            string appId = "d40189c7-0c01-4603-8bc4-49abab8d42e4";
            //string conversationId = "a:1bFx4JBy_BE6ZiKOdNYuX4wg7zvJZzf2KUwj8JSukzzCrJ3JajqTJBx3kwnOXd0sdj6XBMQcw3IwmnVYecqc5_FkT3CPwi2x6YFMFpRMsU7Jnc3mYDyGeVm7kcdV46rtr";
            string conversationId = "29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ";
            //string conversationId = "1064787034";
            var coversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = "28:d40189c7-0c01-4603-8bc4-49abab8d42e4", Name = "oliver_test_bot", Role = "bot" },
                Conversation = new ConversationAccount { Id = conversationId },
                ServiceUrl = "https://smba.trafficmanager.net/apis/",
                ChannelId = "skype"
            };
            await ((CloudAdapter)adapter).ContinueConversationAsync(appId, coversation,
                    async (context, token) =>
                    {
                        var activity = MessageFactory.Text("This is a text message");
                        activity.TextFormat = TextFormatTypes.Markdown;
                        activity.Attachments.Add(new Attachment
                        {
                            ContentType = "image",
                            ContentUrl = "https://letsenhance.io/static/8f5e523ee6b2479e26ecc91b9c25261e/1015f/After.jpg"
                        });
                        await context.SendActivityAsync(activity, cancellationToken: token);
                    },
                    new CancellationToken());

            return Ok();
        }
    }
}