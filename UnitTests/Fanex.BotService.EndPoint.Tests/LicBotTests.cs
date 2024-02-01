namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using NSubstitute;
    using Xunit;

    public class LicBotTests
    {
        private readonly IConversationService conversationService;
        private readonly ITurnContext<IConversationUpdateActivity> turnContextConversation;
        private readonly ITurnContext<IMessageActivity> turnContextMessage;
        private readonly ITurnContext turnContext;

        public LicBotTests()
        {
            conversationService = Substitute.For<IConversationService>();
            turnContextConversation = Substitute.For<ITurnContext<IConversationUpdateActivity>>();
            turnContextMessage = Substitute.For<ITurnContext<IMessageActivity>>();
            turnContext = Substitute.For<ITurnContext>();
        }

        [Fact]
        public void OnConversationUpdateActivityAsync_WhenWrongChannel_JustReplyBack()
        {
            turnContextConversation.Activity.ChannelId.Returns("telegram");
            var channel = new ChannelAccount { Name = "Test" };
            turnContextConversation.Activity.Recipient.Returns(channel);

            SetupLicBot("OnConversationUpdateActivityAsync", turnContextConversation);

            turnContextConversation.Received().SendActivityAsync(Arg.Any<IActivity>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task OnConversationUpdateActivityAsync_WhenAddMember_DoExpectedAction()
        {
            IActivity activity = new Activity
            {
                ChannelId = "skype",
                Recipient = new ChannelAccount { Id = "test", Name = "test" },
                MembersAdded = new List<ChannelAccount>
                {
                    new ChannelAccount { Id = "test", Name = "test1" }
                },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" }
            };

            turnContextConversation.Activity.Returns(activity);
            SetupLicBot("OnConversationUpdateActivityAsync", turnContextConversation);

            await conversationService.Received().AddBotToGroup(Arg.Any<AddBotToGroupCommand>());
        }

        [Fact]
        public async Task OnConversationUpdateActivityAsync_WhenRemoveMember_DoExpectedAction()
        {
            IActivity activity = new Activity
            {
                ChannelId = "skype",
                Recipient = new ChannelAccount { Id = "test", Name = "test" },
                MembersRemoved = new List<ChannelAccount>
                {
                    new ChannelAccount { Id = "test", Name = "test1" }
                },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" }
            };

            turnContextConversation.Activity.Returns(activity);
            SetupLicBot("OnConversationUpdateActivityAsync", turnContextConversation);

            await conversationService.Received().RemoveBotOutGroup(Arg.Any<RemoveBotOutGroupCommand>());
        }

        [Fact]
        public async Task OnConversationUpdateActivityAsync_UpdateToicName_DoExpectedAction()
        {
            IActivity activity = new Activity
            {
                ChannelId = "skype",
                Recipient = new ChannelAccount { Id = "test", Name = "test" },
                MembersRemoved = new List<ChannelAccount>
                {
                    new ChannelAccount { Id = "test", Name = "test1" }
                },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" },
                TopicName = "Topic name"
            };

            turnContextConversation.Activity.Returns(activity);
            SetupLicBot("OnConversationUpdateActivityAsync", turnContextConversation);

            await conversationService.Received().UpdateGroupTopic(Arg.Any<UpdateGroupTopicCommand>());
        }

        [Fact]
        public async Task OnMessageActivityAsync_WithHelpCommand_DoExpectedAction()
        {
            IActivity activity = new Activity
            {
                Text = "@test help",
                Recipient = new ChannelAccount { Id = "abc", Name = "@test" },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" },
                TopicName = "Topic name"
            };

            turnContextMessage.Activity.Returns(activity);
            SetupLicBot("OnMessageActivityAsync", turnContextMessage);

            await turnContextMessage.Received().SendActivityAsync(Arg.Any<IActivity>());
        }

        [Fact]
        public async Task OnMessageActivityAsync_WithGetIdCommand_DoExpectedAction()
        {
            IActivity activity = new Activity
            {
                Text = "@test getid",
                Recipient = new ChannelAccount { Id = "abc", Name = "@test" },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" },
                TopicName = "Topic name"
            };

            turnContextMessage.Activity.Returns(activity);
            SetupLicBot("OnMessageActivityAsync", turnContextMessage);

            await turnContextMessage.Received().SendActivityAsync(Arg.Any<IActivity>());
        }

        [Fact]
        public async Task OnMessageActivityAsync_WithOtherCommand_Donothing()
        {
            IActivity activity = new Activity
            {
                Text = "@test This is text",
                Recipient = new ChannelAccount { Id = "abc", Name = "@test" },
                Conversation = new ConversationAccount { Id = "Testing", IsGroup = true, Name = "Account name" },
                ServiceUrl = "www.test.com",
                From = new ChannelAccount { Id = "test1", Name = "test1" },
                TopicName = "Topic name"
            };

            turnContextMessage.Activity.Returns(activity);
            SetupLicBot("OnMessageActivityAsync", turnContextMessage);

            await turnContextMessage.DidNotReceive().SendActivityAsync(Arg.Any<IActivity>());
        }

        private void SetupLicBot(string methodName, object context)
        {
            var type = typeof(LicBot);
            var licbot = Activator.CreateInstance(type, conversationService);
            MethodInfo method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(x => x.Name == methodName && x.IsVirtual);
            var param = new object[] { context, new CancellationToken() };
            method.Invoke(licbot, param);
        }
    }
}