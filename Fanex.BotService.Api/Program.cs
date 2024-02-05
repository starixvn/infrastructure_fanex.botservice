using Fanex.BotService.Api;
using Fanex.BotService.Shared.Extensions;
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

builder.Services.AddControllers();
builder.Services.AddRazorPages();
DI.RegistryConfigurations(configuration);
builder.Services.RegisterCoreService();
builder.Services.RegisterSentry(configuration);
builder.Services.RegisterRedis(configuration);
builder.Services.RegisterServices(configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();