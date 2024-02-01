namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Fanex.NCB.BotService.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    public class AuthController : ControllerBase
    {
        private readonly AppSettings appSettings;
        private readonly string invalidUsernameOrPasswordText = "Username or password is incorrect";

        public AuthController(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/authenticate")]
        public ObjectResult Authenticate(string username, string password)
        {
            var token = string.Empty;
            if (IsValidCredential(username, password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                token = tokenHandler.WriteToken(sercurityToken);

                return Ok(new { AccessToken = token });
            }
            else
            {
                return BadRequest(invalidUsernameOrPasswordText);
            }
        }

        private bool IsValidCredential(string username, string password)
        {
            return username.Equals(appSettings.AppCredentialId, StringComparison.InvariantCultureIgnoreCase)
                && password.Equals(appSettings.AppCredentialPassword, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}