namespace Fanex.BotService.Api.Filter
{
    using System.IdentityModel.Tokens.Jwt;
    using Fanex.BotService.Api.Criteria;
    using Fanex.BotService.Api.Models;
    using Fanex.BotService.Shared.Extensions;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AppValidationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string jwtToken = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                CloseConnection(context);
            }
            var auth = DecodeJwt(jwtToken);
            if (auth is null)
            {
                CloseConnection(context);
            }

            var baseService = context.HttpContext.RequestServices.GetService<IBaseService>();
            var bot = await baseService.GetDataEntityAsync<AppModal>(new GetAppByIdCriteria { AppId = auth.AppId });
            if (bot is null || !auth.SecretKey.Equals(EncryptSecret(bot.AppPassword, bot.SecretKey)))
            {
                CloseConnection(context);
            }
        }

        private static string EncryptSecret(string password, string secret)
            => Encryption.Encrypt(password, secret);

        private static AuthorizeModel DecodeJwt(string jwtToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);
                var json = token.Payload.SerializeToJson();
                var data = JsonConvert.DeserializeObject<AuthorizeModel>(json);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void CloseConnection(AuthorizationFilterContext context)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}