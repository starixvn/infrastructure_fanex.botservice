namespace Fanex.BotService.EndPoint.Controllers;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Services.Interfaces;

public class AuthController(AppSettings appSettings, ICommonLogService commonLogService) : ControllerBase
{
    private const string InvalidUsernameOrPasswordText = "Username or password is incorrect";

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
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name, username)
                ]),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(new { AccessToken = token, Expires = expires });
        }

        return BadRequest(InvalidUsernameOrPasswordText);
    }

    private bool IsValidCredential(string username, string password)
    {
        return username.Equals(appSettings.AppCredentialId, StringComparison.InvariantCultureIgnoreCase)
               && password.Equals(appSettings.AppCredentialPassword, StringComparison.InvariantCultureIgnoreCase);
    }
}