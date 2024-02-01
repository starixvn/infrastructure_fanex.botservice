namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class RemoveBotOutGroupCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new RemoveBotOutGroupCommand();

            var expected = "NCB_Conversation_RemoveBotOutGroup";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", "1")]
        [InlineData("1", "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string botConversationId, string conversationId)
        {
            // Arrange
            var command = new RemoveBotOutGroupCommand
            {
                ConversationId = conversationId,
                BotConversationId = botConversationId
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
            var command = new RemoveBotOutGroupCommand
            {
                ConversationId = "aa",
                BotConversationId = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}