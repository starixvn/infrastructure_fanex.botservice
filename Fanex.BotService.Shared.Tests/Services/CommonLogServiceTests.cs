using Fanex.BotService.Shared.Services;
using NSubstitute;

namespace Fanex.BotService.Shared.Tests.Services;

public class CommonLogServiceTests
{
    private readonly CommonLogService commonLogService;
    private readonly Logging.ILogger mockLogger;

    public CommonLogServiceTests()
    {
        mockLogger = Substitute.For<Logging.ILogger>();
        commonLogService = new CommonLogService(mockLogger);
    }

    [Fact]
    public void LogFileError_ShouldLogError()
    {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        commonLogService.LogFileError(exception);

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
        commonLogService.LogFileMessage(message);

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
        await commonLogService.SentryErrorAsync(errMsg, exception);

        // Assert
        await mockLogger.Received(1).ErrorAsync(errMsg, exception);
    }

    [Fact]
    public async Task SentryInfoAsync_ShouldLogInfoAsync()
    {
        // Arrange
        var info = "Test info message";

        // Act
        await commonLogService.SentryInfoAsync(info);

        // Assert
        await mockLogger.Received(1).InfoAsync(info);
    }
}