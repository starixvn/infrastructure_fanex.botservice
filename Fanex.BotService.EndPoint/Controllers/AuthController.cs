namespace Fanex.BotService.EndPoint.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Fanex.BotService.Core;
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
            if (IsValidCredential(username, password))
            {
                DateTime? expires = DateTimeOffset.UtcNow.DateTime.AddMinutes(appSettings.JwtLifeTime);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = expires,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var sercurityToken = tokenHandler.CreateToken(tokenDescriptor);
                string token = tokenHandler.WriteToken(sercurityToken);
                return Ok(new { AccessToken = token, Expires = expires });
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