using Fanex.BotService.Shared.Middlewares;
using Fanex.Logging;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Fanex.BotService.Shared.Tests.Middlewares;

public class ErrorHandlingMiddlewareTests
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly ErrorHandlingMiddleware _middleware;

    public ErrorHandlingMiddlewareTests()
    {
        _next = Substitute.For<RequestDelegate>();
        _logger = Substitute.For<ILogger>();
        _middleware = new ErrorHandlingMiddleware(_next, _logger);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNextDelegate()
    {
        var context = new DefaultHttpContext();

        await _middleware.InvokeAsync(context);

        await _next.Received(1).Invoke(context);
    }

    [Fact]
    public async Task InvokeAsync_ShouldHandleException()
    {
        var context = new DefaultHttpContext
        {
            Request =
            {
                Method = "post",
                Headers =
                {
                    ["X-Requested-With"] = "XMLHttpRequest"
                }
            }
        };

        var exception = new Exception("Test exception");
        _next.When(x => x.Invoke(context)).Throw(exception);

        await _middleware.InvokeAsync(context);

        await _logger.Received(1).ErrorAsync(Arg.Is<string>(msg => msg.Contains("Test exception")));
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }
}