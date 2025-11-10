namespace Fanex.BotService.EndPoint;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using Bots;
using Bots.Interfaces;
using Caching;
using Caching.Redis;
using Core;
using Core.Models;
using Core.Services;
using Core.Services.Interfaces;
using Data.Repository;
using Data.SqlServer;
using Logging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Repositories;
using Shared.Services;
using Shared.Services.Interfaces;
using StackExchange.Redis;

public static class ServiceInjectorExtensions
{
    public static void Init(IServiceCollection services)
    {
        services.AddTransient<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
        services.AddTransient<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
        services.AddTransient<ServiceClientCredentialsFactory, MultiServiceClientCredentialsFactory>();
        services.AddTransient<HttpClient, CustomHttpClient>();

        services.AddTransient<ITeamsBot, TeamsBot>();
        services.AddTransient<ISkypeBot, SkypeBot>();
        services.AddTransient<ITelegramBot, TelegramBot>();
        services.AddTransient<ICoreBot, CoreBot>();
        services.AddTransient<IBot, BotFactory>();

        services.AddScoped<Func<string, IChannelBot>>(serviceProvider => (channelType) =>
        {
            switch (channelType)
            {
                case "msteams":
                    return serviceProvider.GetService<ITeamsBot>();

                case "skype":
                    return serviceProvider.GetService<ISkypeBot>();

                case "telegram":
                    return serviceProvider.GetService<ITelegramBot>();

                default:
                    return serviceProvider.GetService<ICoreBot>();
            }
        });
    }

    public static void RegisterService(IServiceCollection services, IConfiguration configuration)
    {
        _ = bool.TryParse(configuration.GetSection("IsReadSettingFromFile")?.Value, out var isReadSettingFromFile);

        ConfigHttpClient(services, configuration);
        services.Configure<ProxySettings>(options => configuration.GetSection("ProxySettings").Bind(options));

        if (isReadSettingFromFile)
        {
            var botConfigsSection = configuration.GetSection("BotConfigs");
            var botConfigs = botConfigsSection.Get<IList<BotConfigs>>();
            services.AddSingleton(botConfigs);
            services.AddSingleton<IConversationService, FakeConversationService>();
            services.AddSingleton<IApplicationSettingService, FakeApplicationSettingService>();
        }
        else
        {
            services.AddFanexSql(configuration);
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IConversationService, ConversationService>();
            services.AddSingleton<IApplicationSettingService, ApplicationSettingService>();
        }

        services.AddSingleton<IMessageValidationService, MessageValidationService>();
        services.AddTransient<IMessageSenderService, MessageSenderService>();
        services.AddTransient<ILogger, Logger>();
        services.AddTransient<ICommonLogService, CommonLogService>();

        RegisterCheckDuplicateMessageLogSettings(services, configuration);
    }

    public static void RegisterCache(IServiceCollection services, IConfiguration configuration)
    {
        _ = bool.TryParse(configuration.GetSection("IsCacheDataInRedis")?.Value, out var isCacheDataInRedis);
        services.AddTransient<ICacheService, CacheService>();
        services.AddSingleton<IMemoryCache, MemoryCache>();
        if (isCacheDataInRedis)
        {
            var multiplexer = ConnectionMultiplexer.Connect(configuration.GetSection("Fanex.Caching.Redis:ConnectionString").Value ?? string.Empty);
            services.AddDataProtection()
                .SetApplicationName(configuration.GetSection("Fanex.Caching.Redis:ApplicationName").Value ?? string.Empty)
                .PersistKeysToStackExchangeRedis(multiplexer, "Fanex.ChatBotBotService.Endpoint-Keys");
            services.AddRedisCache(configuration.GetSection("Fanex.Caching.Redis"));

            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }
        else
        {
            services.AddMemoryCache();
        }
    }

    private static void RegisterCheckDuplicateMessageLogSettings(IServiceCollection services, IConfiguration configuration)
    {
        var settingConfiguration = configuration.GetSection("CheckDuplicateMessageLogSettings");
        services.Configure<CheckDuplicatedMessageLogSetting>(settingConfiguration);
        var checkDuplicatedMessageSettings = settingConfiguration.Get<CheckDuplicatedMessageLogSetting>();
        if (checkDuplicatedMessageSettings != null) services.AddSingleton(checkDuplicatedMessageSettings);
    }

    private static void ConfigHttpClient(IServiceCollection services, IConfiguration configuration)
    {
        var useProxyValue = configuration.GetSection("ProxySettings:IsUseProxy").Value;
        var isUseProxy = useProxyValue != null && bool.Parse(useProxyValue);
        if (isUseProxy)
        {
            var proxyAddress = configuration.GetSection("ProxySettings:ProxyAddress").Value;
            var value = configuration.GetSection("ProxySettings:BypassOnLocal").Value;
            var bypassOnLocal = value != null && bool.Parse(value);

            services.AddHttpClient(NamedHttpClients.ProxiedClient)
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var proxy = new WebProxy(proxyAddress, bypassOnLocal);
                    return new SocketsHttpHandler
                    {
                        UseProxy = true,
                        Proxy = proxy,
                        SslOptions = new SslClientAuthenticationOptions
                        {
                            EnabledSslProtocols = SslProtocols.Tls12
                        }
                    };
                });
        }
        else
        {
            services.AddHttpClient();
        }
    }
}

public class CustomHttpClient : HttpClient
{
    public CustomHttpClient(IConfiguration configuration)
    {
        var isUseProxy = bool.Parse(configuration.GetSection("ProxySettings:IsUseProxy").Value ?? string.Empty);
        var proxyAddress = configuration.GetSection("ProxySettings:ProxyAddress").Value;
        var bypassOnLocal = bool.Parse(configuration.GetSection("ProxySettings:BypassOnLocal").Value ?? string.Empty);

        if (isUseProxy)
        {
            var proxy = new WebProxy(proxyAddress, bypassOnLocal);
            DefaultProxy = proxy;
        }
    }
}