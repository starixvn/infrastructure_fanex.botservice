using Fanex.BotService.Client.Core.Client;
using Fanex.BotService.Client.Core.Models;
using NSubstitute;

namespace Fanex.BotService.Client.Core.Tests.Client;

public class BotClientTests
{
    private readonly BotClient botClient;
    private readonly BaseHttpClient baseHttpClient;

    public BotClientTests()
    {
        baseHttpClient = Substitute.For<BaseHttpClient>();
        botClient = new BotClient();
    }

    [Fact]
    public async Task SendSkypeAsync_ShouldCallPostAsyncWithCorrectParameters()
    {
        // Arrange
        var conversation = new Conversation { ConversationId = "123", Content = "Test" };

        // Act
        await botClient.SendSkypeAsync(conversation);

        // Assert
        await baseHttpClient.Received(1).PostAsync<Conversation, Task>("api/skype/send", conversation);
    }

    [Fact]
    public async Task SendTelegramAsync_ShouldCallPostAsyncWithCorrectParameters()
    {
        // Arrange
        var conversation = new Conversation { ConversationId = "123", Content = "Test" };

        // Act
        await botClient.SendTelegramAsync(conversation);

        // Assert
        await baseHttpClient.Received(1).PostAsync<Conversation, Task>("api/telegram/send", conversation);
    }

    [Fact]
    public async Task SendMsTeamsAsync_ShouldCallPostAsyncWithCorrectParameters()
    {
        // Arrange
        var conversation = new Conversation { ConversationId = "123", Content = "Test" };

        // Act
        await botClient.SendMsTeamsAsync(conversation);

        // Assert
        await baseHttpClient.Received(1).PostAsync<Conversation, Task>("api/bot/send", conversation);
    }

    [Fact]
    public async Task SendAsync_ShouldCallPostAsyncWithCorrectParameters()
    {
        // Arrange
        var conversation = new Conversation { ConversationId = "123", Content = "Test" };

        // Act
        await botClient.SendAsync(conversation);

        // Assert
        await baseHttpClient.Received(1).PostAsync<Conversation, Task>("api/bot/send", conversation);
    }
}