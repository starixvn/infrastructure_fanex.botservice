using System.Text;
using System.Threading;
using Fanex.BotService.Core;
using Fanex.BotService.EndPoint;
using Fanex.BotService.Shared.Extensions;
using Fanex.BotService.Shared.Middlewares;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((_, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
});

builder.WebHost
    .UseNLog();

IConfiguration configuration = builder.Configuration;
CoreServiceCollectionExtensions.RegisterSentry(configuration);

var services = builder.Services;

services.AddControllers();
services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var appSettingsSection = configuration.GetSection("AppSettings");
var appSettings = appSettingsSection.Get<AppSettings>();
services.AddSingleton(appSettings);

var key = Encoding.ASCII.GetBytes(appSettings.Secret);
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

ServiceInjectorExtensions.Init(services);

ServiceInjectorExtensions.RegisterService(services, configuration);
ServiceInjectorExtensions.RegisterCache(services, configuration);
services.AddControllers(x => x.EnableEndpointRouting = false);
IdentityModelEventSource.ShowPII = true;
const short workerThreads = 300;
const short completionPortThreads = 300;
_ = ThreadPool.SetMinThreads(workerThreads, completionPortThreads);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseForwardedHeaders();
}
else
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseForwardedHeaders();
    app.UseHsts();
}

var domainPath = configuration.GetValue<string>("AppSettings:DomainPath");
if (!string.IsNullOrWhiteSpace(domainPath))
{
    app.UsePathBase(new PathString(domainPath));
    app.Use(async (ctx, next) =>
    {
        ctx.Request.PathBase = domainPath;
        await next();
    });
}

app.UseAuthorization();
app.UseAuthentication();
app.MapGet("/", () => "Fanex Bot Endpoint ready!");
app.MapGet("/test-sentry", async () =>
{
    var commonLogService = app.Services.GetRequiredService<ICommonLogService>();
    commonLogService.LogFileMessage("Test log message");

    await commonLogService.SentryErrorAsync("Test Sentry Error from /test-sentry endpoint");
});

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();