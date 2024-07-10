namespace Fanex.BotService.EndPoint
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using Fanex.BotService.Core;
    using Fanex.BotService.Core.Models;
    using Fanex.BotService.Core.Services;
    using Fanex.BotService.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Extensions;
    using Fanex.BotService.Shared.Services;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.Caching;
    using Fanex.Caching.Redis;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Fanex.Logging;
    using Fanex.NCB.Shared.Middlewares;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.IdentityModel.Tokens;
    using StackExchange.Redis;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            CoreServiceCollectionExtensions.RegisterSentry(Configuration);

            var appSettingsSection = Configuration.GetSection("AppSettings");
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

            BotServiceDI.Init(services);

            RegisterService(services, Configuration);
            RegisterCache(services, Configuration);
            services.AddControllers(x => x.EnableEndpointRouting = false);
            IdentityModelEventSource.ShowPII = true;
            const short WorkerThreads = 300;
            const short CompletionPortThreads = 300;
            _ = ThreadPool.SetMinThreads(WorkerThreads, CompletionPortThreads);
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
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

            app.UseDefaultFiles();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Default",
                    "/{controller}/{action}",
                    new { controller = "Home", action = "Index" });
            });
        }

        private static void RegisterService(IServiceCollection services, IConfiguration configuration)
        {
            _ = bool.TryParse(configuration.GetSection("IsReadSettingFromFile")?.Value, out bool isReadSettingFromFile);

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
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ICommonLogService, CommonLogService>();

            RegisterCheckDuplicateMessageLogSettings(services, configuration);
        }

        private static void RegisterCache(IServiceCollection services, IConfiguration configuration)
        {
            _ = bool.TryParse(configuration.GetSection("IsCacheDataInRedis")?.Value, out bool isCacheDataInRedis);
            services.AddTransient<ICacheService, CacheService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            if (isCacheDataInRedis)
            {
                var multiplexer = ConnectionMultiplexer.Connect(configuration.GetSection("Fanex.Caching.Redis:ConnectionString").Value);
                services.AddDataProtection()
                .SetApplicationName(configuration.GetSection("Fanex.Caching.Redis:ApplicationName").Value)
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
            services.AddSingleton(checkDuplicatedMessageSettings);
        }

        private static void ConfigHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            var isUseProxy = bool.Parse(configuration.GetSection("ProxySettings:IsUseProxy").Value);
            if (isUseProxy)
            {
                var proxyAddress = configuration.GetSection("ProxySettings:ProxyAddress").Value;
                var bypassOnLocal = bool.Parse(configuration.GetSection("ProxySettings:BypassOnLocal").Value);
                services.AddHttpClient(NamedHttpClients.ProxiedClient)
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var proxy = new WebProxy(proxyAddress, bypassOnLocal);
                        return new SocketsHttpHandler
                        {
                            UseProxy = isUseProxy,
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
}