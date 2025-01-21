using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria
{
    public class AddBotContactCommandTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var command = new AddBotContactCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            Assert.Equal("Conversation_AddContact", result);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenAllRequiredFieldsAreProvided()
        {
            // Arrange
            var command = new AddBotContactCommand
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
        public void IsValid_ShouldReturnFalse_WhenAnyRequiredFieldIsMissing()
        {
            // Arrange
            var command = new AddBotContactCommand
            {
                BotConversationId = "botConversationId",
                ConversationId = "conversationId",
                ChannelName = null // Missing required field
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}