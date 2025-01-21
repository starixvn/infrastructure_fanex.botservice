using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria
{
    public class UpdateGroupTopicCommandTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            Assert.Equal("Conversation_UpdateGroupTopic", result);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenConversationIdAndGroupNameAreNotEmpty()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = "123",
                GroupName = "TestGroup"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenConversationIdIsEmpty()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = "",
                GroupName = "TestGroup"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenGroupNameIsEmpty()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = "123",
                GroupName = ""
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}