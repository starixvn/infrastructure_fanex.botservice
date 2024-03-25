using Fanex.BotService.Comsumer;
using Fanex.BotService.Shared.Extensions;
using Fanex.Data.SqlServer;
using Fanex.NCB.Shared.Middlewares;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls($"http://*:5000")
    .UseNLog();
IConfiguration configuration = builder.Configuration;

// Register core services
builder.Services.AddControllers();
CoreServiceCollectionExtensions.RegisterSentry(configuration);
builder.Services.AddFanexSql(configuration);

//Register services
DI.RegisterConfiguration(configuration);
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

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();