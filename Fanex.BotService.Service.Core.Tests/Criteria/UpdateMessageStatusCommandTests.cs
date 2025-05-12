using Fanex.BotService.Service.Core.Criteria.Message;

namespace Fanex.BotService.Core.Tests.Criteria;

public class UpdateMessageStatusCommandTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectKey()
    {
        // Arrange
        var command = new UpdateMessageStatusCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        Assert.Equal("MessageStatus_Update", result);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenMessageIdIsGreaterThanZeroAndStatusIsNotEmpty()
    {
        // Arrange
        var command = new UpdateMessageStatusCommand
        {
            MessageId = 1,
            Status = "Sent"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenMessageIdIsZero()
    {
        // Arrange
        var command = new UpdateMessageStatusCommand
        {
            MessageId = 0,
            Status = "Sent"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenStatusIsEmpty()
    {
        // Arrange
        var command = new UpdateMessageStatusCommand
        {
            MessageId = 1,
            Status = ""
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }
}