namespace Fanex.NCB.Shared.Tests.Middlewares
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.Shared.Middlewares;
    using Microsoft.AspNetCore.Http;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    public class ErrorHandlingMiddlewareTests
    {
        private RequestDelegate next;
        private ILogger logger;

        [Fact]
        public async Task WhenMiddleware_NotThrowException()
        {
            var middleware = SetupMiddleware();
            var httpContext = Substitute.For<HttpContext>();
            //Actual
            await middleware.InvokeAsync(httpContext);

            await next.Received().Invoke(Arg.Any<HttpContext>());
            logger.DidNotReceive().Error(Arg.Any<string>());
        }

        [Fact]
        public async Task WhenMiddleware_WhenThrowException_AndRedirect()
        {
            var middleware = SetupMiddleware();
            var httpContext = Substitute.For<HttpContext>();
            next(Arg.Any<HttpContext>()).Throws(new Exception());
            //Actual
            await middleware.InvokeAsync(httpContext);

            await logger.Received().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task WhenMiddleware_WhenThrowException_ReturnErrorCode()
        {
            var middleware = SetupMiddleware();
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method = "post";
            httpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
            next(Arg.Any<HttpContext>()).Throws(new Exception());
            //Actual
            await middleware.InvokeAsync(httpContext);

            Assert.Equal(500, httpContext.Response.StatusCode);
        }

        private ErrorHandlingMiddleware SetupMiddleware()
        {
            next = Substitute.For<RequestDelegate>();
            logger = Substitute.For<ILogger>();
            return new ErrorHandlingMiddleware(next, logger);
        }
    }
}