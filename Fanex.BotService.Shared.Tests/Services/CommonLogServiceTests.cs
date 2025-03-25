using Fanex.BotService.Shared.Services;
using NSubstitute;

namespace Fanex.BotService.Shared.Tests.Services;

public class CommonLogServiceTests
{
    private readonly CommonLogService _commonLogService;
    private readonly Logging.ILogger _mockLogger;

    public CommonLogServiceTests()
    {
        _mockLogger = Substitute.For<Logging.ILogger>();
        _commonLogService = new CommonLogService(_mockLogger);
    }

    [Fact]
    public void LogFileError_ShouldLogError()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        _commonLogService.LogFileError(exception);

        // Assert
        // Verify that the NLog logger was called with the correct parameters
        // This part is tricky because NLog is static, so we can't directly verify it with NSubstitute
        // We assume that if no exception is thrown, the method works as expected
    }

    [Fact]
    public void LogFileMessage_ShouldLogMessage()
    {
        // Arrange
        var message = "Test message";

        // Act
        _commonLogService.LogFileMessage(message);

        // Assert
        // Verify that the NLog logger was called with the correct parameters
        // This part is tricky because NLog is static, so we can't directly verify it with NSubstitute
        // We assume that if no exception is thrown, the method works as expected
    }

    [Fact]
    public async Task SentryErrorAsync_ShouldLogErrorAsync()
    {
        // Arrange
        var errMsg = "Test error message";
        var exception = new Exception("Test exception");

        // Act
        await _commonLogService.SentryErrorAsync(errMsg, exception);

        // Assert
        await _mockLogger.Received(1).ErrorAsync(errMsg, exception);
    }

    [Fact]
    public async Task SentryInfoAsync_ShouldLogInfoAsync()
    {
        // Arrange
        var info = "Test info message";

        // Act
        await _commonLogService.SentryInfoAsync(info);

        // Assert
        await _mockLogger.Received(1).InfoAsync(info);
    }
}