using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria
{
    public class AddBotToGroupCommandTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var command = new AddBotToGroupCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            Assert.Equal("Conversation_AddBotToGroup", result);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenAllRequiredFieldsAreProvided()
        {
            // Arrange
            var command = new AddBotToGroupCommand
            {
                BotConversationId = "botConversationId",
                ConversationId = "conversationId",
                ChannelName = "channelName"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenBotConversationIdIsMissing()
        {
            // Arrange
            var command = new AddBotToGroupCommand
            {
                ConversationId = "conversationId",
                ChannelName = "channelName"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenConversationIdIsMissing()
        {
            // Arrange
            var command = new AddBotToGroupCommand
            {
                BotConversationId = "botConversationId",
                ChannelName = "channelName"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenChannelNameIsMissing()
        {
            // Arrange
            var command = new AddBotToGroupCommand
            {
                BotConversationId = "botConversationId",
                ConversationId = "conversationId"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}