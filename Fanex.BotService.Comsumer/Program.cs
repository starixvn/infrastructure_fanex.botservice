using Fanex.BotService.Comsumer;
using Fanex.BotService.Shared.Extensions;
using Fanex.Data.SqlServer;
using Fanex.NCB.Shared.Middlewares;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseUrls($"http://*:5000")
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
    })
    .UseNLog();
IConfiguration configuration = builder.Configuration;

// Register core services
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.RegisterCoreService();
builder.Services.RegisterSentry(configuration);
builder.Services.RegisterRedis(configuration);
builder.Services.AddFanexSql(configuration);
builder.Services.RegisterHttpClient(configuration);

//Register services
DI.RegisterConfiguration(configuration);
builder.Services.RegisterServices();

//Start message broker
builder.Services.StartMessageBroker();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseHsts();
}

app.UseHttpsRedirection();
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