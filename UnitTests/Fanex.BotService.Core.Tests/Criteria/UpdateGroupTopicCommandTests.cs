namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class UpdateGroupTopicCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = string.Empty,
                GroupName = string.Empty,
                UpdatedBy = string.Empty
            };

            var expected = "NCB_Conversation_UpdateGroupTopic";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_ConversationIdIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = string.Empty,
                GroupName = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_GroupNameIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = "aa",
                GroupName = string.Empty
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = "aa",
                GroupName = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
