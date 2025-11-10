using Fanex.BotService.Consumer;
using Fanex.BotService.Shared.Extensions;
using Fanex.BotService.Shared.Middlewares;
using Fanex.BotService.Shared.Services.Interfaces;
using Fanex.Data.SqlServer;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseNLog();
IConfiguration configuration = builder.Configuration;

// Register core services
CoreServiceCollectionExtensions.RegisterSentry(configuration);
builder.Services.AddFanexSql(configuration);

//Register services
Di.RegisterConfiguration(configuration);
builder.Services.RegisterHttpClient(configuration);
builder.Services.RegisterServices();

//Start message broker
builder.Services.StartMessageBroker();

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

app.MapGet("/", () => "Fanex Bot consumer running ...");
app.MapGet("/sentry", async () =>
{
    var logService = app.Services.GetRequiredService<ICommonLogService>();
    await logService.SentryErrorAsync("Test Sentry from Consumer");

    return "Test sentry!";
});

await app.RunAsync();