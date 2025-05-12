using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria;

public class RemoveBotOutGroupCommandTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectKey()
    {
        // Arrange
        var command = new RemoveBotOutGroupCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        Assert.Equal("Conversation_RemoveBotOutGroup", result);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenBotConversationIdAndConversationIdAreNotEmpty()
    {
        // Arrange
        var command = new RemoveBotOutGroupCommand
        {
            BotConversationId = "bot123",
            ConversationId = "conv123"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenBotConversationIdIsEmpty()
    {
        // Arrange
        var command = new RemoveBotOutGroupCommand
        {
            BotConversationId = string.Empty,
            ConversationId = "conv123"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenConversationIdIsEmpty()
    {
        // Arrange
        var command = new RemoveBotOutGroupCommand
        {
            BotConversationId = "bot123",
            ConversationId = string.Empty
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }
}