using Fanex.BotService.Core;
using Fanex.BotService.EndPoint.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Tests.Controllers;

using Shared.Services.Interfaces;

public class AuthControllerTests
{
    private readonly AuthController _authController;
    private readonly ICommonLogService logService;

    public AuthControllerTests()
    {
        var appSettings = new AppSettings
        {
            AppCredentialId = "testUser",
            AppCredentialPassword = "testPassword",
            Secret = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ",
            JwtLifeTime = 30
        };

        var options = Substitute.For<IOptions<AppSettings>>();
        options.Value.Returns(appSettings);
        logService = Substitute.For<ICommonLogService>();
        _authController = new AuthController(options.Value, logService);
    }

    [Fact]
    public async Task SentryTest_ReturnsOkResult()
    {
        // Act
        var result = await _authController.TestSentry() as OkObjectResult;
        // Assert
        Assert.NotNull(result);
        await logService.Received(1).SentryErrorAsync(Arg.Is("Test Sentry"), Arg.Any<Exception>());
        Assert.Equal("Sentry test logged", result.Value);
    }

    [Fact]
    public void Authenticate_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var username = "testUser";
        var password = "testPassword";

        // Act
        var result = _authController.Authenticate(username, password) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Authenticate_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var username = "invalidUser";
        var password = "invalidPassword";

        // Act
        var result = _authController.Authenticate(username, password) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Username or password is incorrect", result.Value);
    }
}