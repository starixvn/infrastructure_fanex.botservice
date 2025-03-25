using Fanex.BotService.Api.Controllers;
using Fanex.BotService.Service.Core.Enum;
using Fanex.BotService.Service.Core.Models;
using Fanex.BotService.Service.Core.Services.Interfaces;
using Fanex.BotService.Shared.Models.Message;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanex.BotService.Api.Tests.Controllers;

public class MessageControllerTests
{
    private readonly IMessageQueueService _queueService;
    private readonly MessageController _messageController;

    public MessageControllerTests()
    {
        _queueService = Substitute.For<IMessageQueueService>();
        _messageController = new MessageController(_queueService);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldReturnOk_WhenConversationIsValid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Hello",
            Images = ["image1.png"]
        };

        // Act
        var result = await _messageController.SendMessageAsync(BotChannel.Skype, conversation);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        await _queueService.Received(1).PublishAsync(conversation);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldReturnBadRequest_WhenConversationIsInvalid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 0,
            ConversationId = "",
            Content = "",
            Images = ["image1.png"]
        };

        // Act
        var result = await _messageController.SendMessageAsync(BotChannel.Skype, conversation);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid input data", badRequestResult.Value);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnOk_WhenConversationIsValid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Hello",
            Images = ["image1.png"]
        };

        // Act
        var result = await _messageController.SendAsync(conversation);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        await _queueService.Received(1).PublishAsync(conversation);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnBadRequest_WhenConversationIsInvalid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 0,
            ConversationId = "",
            Content = "",
            Images = ["image1.png"]
        };

        // Act
        var result = await _messageController.SendAsync(conversation);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid input data", badRequestResult.Value);
    }

    [Fact]
    public async Task RetryMessageAsync_ShouldReturnOk_WhenMessageEndpointIsValid()
    {
        // Arrange
        var message = new MessageEndpoint
        {
            MessageId = 1,
            AppId = 1,
            ConversationId = "123",
            Content = "Hello"
        };

        // Act
        var result = await _messageController.RetryMessageAsync(message);

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
        await _queueService.Received(1).PublishAsync(message);
    }

    [Fact]
    public async Task RetryMessageAsync_ShouldReturnBadRequest_WhenMessageEndpointIsInvalid()
    {
        // Arrange
        var message = new MessageEndpoint
        {
            MessageId = 0,
            AppId = 0,
            ConversationId = "",
            Content = ""
        };

        // Act
        var result = await _messageController.RetryMessageAsync(message);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Invalid message ID: {JsonConvert.SerializeObject(message)}", badRequestResult.Value);
    }

    [Fact]
    public void IsValidAttachments_ShouldReturnTrue_WhenAttachmentsAreValid()
    {
        // Arrange
        var attachments = new List<string> { "image1.png", "image2.png" };

        // Act
        var result = MessageController.IsValidAttachments(attachments);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidAttachments_ShouldReturnFalse_WhenAttachmentsExceedMaxLimit()
    {
        // Arrange
        var attachments = new List<string>();
        for (var i = 0; i < 21; i++)
        {
            attachments.Add($"image{i}.png");
        }

        // Act
        var result = MessageController.IsValidAttachments(attachments);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidConversation_ShouldReturnTrue_WhenConversationIsValid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Hello",
            Images = ["image1.png"]
        };

        // Act
        var result = MessageController.IsValidConversation(conversation, out var errMsg);

        // Assert
        Assert.True(result);
        Assert.Equal("Invalid input data", errMsg);
    }

    [Fact]
    public void IsValidConversation_ShouldReturnFalse_WhenConversationIsInvalid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 0,
            ConversationId = "",
            Content = "",
            Images = ["image1.png"]
        };

        // Act
        var result = MessageController.IsValidConversation(conversation, out var errMsg);

        // Assert
        Assert.False(result);
        Assert.Equal("Invalid input data", errMsg);
    }

    [Fact]
    public void IsValidConversationModel_ShouldReturnTrue_WhenConversationModelIsValid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 1,
            ConversationId = "123",
            Content = "Hello"
        };

        // Act
        var result = MessageController.IsValidConversationModel(conversation);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidConversationModel_ShouldReturnFalse_WhenConversationModelIsInvalid()
    {
        // Arrange
        var conversation = new Conversation
        {
            AppId = 0,
            ConversationId = "",
            Content = ""
        };

        // Act
        var result = MessageController.IsValidConversationModel(conversation);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidMessageEndpoint_ShouldReturnTrue_WhenMessageEndpointIsValid()
    {
        // Arrange
        var message = new MessageEndpoint
        {
            MessageId = 1,
            AppId = 1,
            ConversationId = "123",
            Content = "Hello"
        };

        // Act
        var result = MessageController.IsValidMessageEndpoint(message);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidMessageEndpoint_ShouldReturnFalse_WhenMessageEndpointIsInvalid()
    {
        // Arrange
        var message = new MessageEndpoint
        {
            MessageId = 0,
            AppId = 0,
            ConversationId = "",
            Content = ""
        };

        // Act
        var result = MessageController.IsValidMessageEndpoint(message);

        // Assert
        Assert.False(result);
    }
}