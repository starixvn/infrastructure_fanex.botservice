using Fanex.BotService.Client.AspNetCore;
using Fanex.BotService.Client.Core;
using Fanex.BotService.Client.Core.Configuration;
using Fanex.BotService.Client.Core.Services;
using Fanex.BotService.Client.Core.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddRazorPages();
builder.Services.AddSingleton<IBotSettings, BotSettings>();
builder.Services.AddSingleton<IBotService, BotService>();

var app = builder.Build();
BotClientManager.RegistryBot(new BotSettings(builder.Configuration));
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();