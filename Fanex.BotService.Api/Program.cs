using Fanex.BotService.Api;
using Fanex.BotService.Shared.Extensions;
using Fanex.BotService.Shared.Middlewares;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost
    .UseNLog();

IConfiguration configuration = builder.Configuration;

builder.Services.AddControllers();

CoreServiceCollectionExtensions.RegisterSentry(configuration);

builder.Services.RegisterServices(configuration);

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

app.MapGet("/", () => "Fanex Bot API ready!");
app.MapGet("/sentry", async () =>
{
    var logService = app.Services.GetRequiredService<Fanex.Logging.ILogger>();
    await logService.ErrorAsync("Test Sentry");

    return "Test sentry!";
});

await app.RunAsync();