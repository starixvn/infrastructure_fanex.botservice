namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private AppSettings appSettings;

        [Fact]
        public void Authenticate_InvalidUserOrPassword_ReturnBadRequestResult()
        {
            var controller = GetController();
            
            var userName = string.Empty;
            var password = string.Empty;

            appSettings.AppCredentialId = "AppCredentialId";
            appSettings.AppCredentialPassword = "AppCredentialPassword";

            //Actual
            var actual = controller.Authenticate(userName, password);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.Equal("Username or password is incorrect", badRequestResult.Value);
        }

        [Fact]
        public void Authenticate_ValidUserAndPassword_ReturnOkObjectResult()
        {
            var controller = GetController();

            var userName = "ChatBotForLicensee";
            var password = "ZSI6IjMiLCJuYmYiOjE1";

            appSettings.AppCredentialId = "ChatBotForLicensee";
            appSettings.AppCredentialPassword = "ZSI6IjMiLCJuYmYiOjE1";
            appSettings.Secret = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ";

            //Actual
            var actual = controller.Authenticate(userName, password);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
        }

        private AuthController GetController()
        {
            appSettings = Substitute.For<AppSettings>();
            return new AuthController(appSettings);
        }
    }
}
