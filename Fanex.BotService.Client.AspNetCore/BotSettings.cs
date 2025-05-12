namespace Fanex.BotService.Client.AspNetCore;

using Core.Configuration;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

public class BotSettings : IBotSettings
{
    private readonly IConfiguration configuration;

    public BotSettings(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        AppId = GetValue<int>("Fanex.BotService:AppId");
        ServiceUrl = GetValue<string>("Fanex.BotService:ServiceUrl") ?? throw new ArgumentNullException(ServiceUrl);
    }

    public int AppId { get; set; }

    public string ServiceUrl { get; set; }

    private T GetValue<T>(string key)
    {
        var value = configuration.GetSection(key).Value;
        var typeConverter = TypeDescriptor.GetConverter(typeof(T));
        return (T)typeConverter.ConvertFromString(value);
    }
}