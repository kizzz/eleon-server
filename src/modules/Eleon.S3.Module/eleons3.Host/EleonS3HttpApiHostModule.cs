using Authorization.Module.TenantHostname;
using Common.EventBus.Module;
using Common.Module.Constants;
using Eleoncore.SDK.RequestLocalization;
using Eleoncore.SDK.TenantSettings;
using EleonS3.Application.EventHandlers;
using EleonS3.Domain;
using EleonS3.HttpApi;
using EleonS3.HttpApi.SigV4;
using EleonS3Module;
using EleonsoftAbp.MultiTenancy;
using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Jobs;
using EleonsoftSdk.Auth;
using EleonsoftSdk.Middlewares;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EleonsoftSdk.modules.Helpers.Module;
using EleonsoftSdk.modules.Jobs.Module;
using EleonsoftSdk.modules.StorageProvider.Module;
using Logging.Module;
using Logging.Module.ErrorHandling.Extensions;
using Eleon.Logging.Lib.VportalLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;
using ProxyRouter.Minimal.HttpApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.Ui.LayoutHooks;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Uow;
using Volo.Abp.VirtualFileSystem;
using VPortal.Policies;

namespace VPortal;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(CommonEventBusModule),
    typeof(AbpMultiTenancyModule),
    typeof(EleonS3ModuleCollector)
    )]
public class EleonS3HttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<EleonS3HttpApiHostModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<EleonS3HttpApiHostModule>(validate: true);
        });

        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        context.Services.AddVportalLogging(configuration);

        context.Services
            .AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

            });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = false;
            options.SaveStaticPermissionsToDatabase = false;
        });

        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });

        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });

        context.Services.AddEleonsoftMultiTenancy(configuration);
        context.Services.AddHttpClient();
        context.Services.AddTestTelegramNotificationJob("EleonS3");
        context.Services.AddSendNotificationJob("EleonS3");


        ConfigureAntiforgery();
        ConfigureApplicationCookie(context.Services, configuration);
        ConfigureTenentResolvement(configuration);
        ConfigureUrls(configuration);
        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureLocalizationOptions();
        ConfigureCache(configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureCors(context, configuration);
        ConfigureExternalProviders(context);
        ConfigureHealthChecks(context, configuration);
        ConfigureAuditing();
        ConfigureJobs();
        ConfigureRedis(context, configuration, hostingEnvironment);
        ConfigureAuthorization(context.Services);
        ConfigureAntiforgery(context.Services);
        ConfigureLayoutHooks();
        ConfigureExceptionHandling(context.Services, configuration);

        context.Services.AddProxyRouter();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var cfg = context.GetConfiguration();
        var pathBase = cfg["App:PathBase"] ?? string.Empty;

        app.UseExceptionHandlingMiddleware();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto
        };
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        app.UseForwardedHeaders(options);

        app.UseMiniProfiler();
        app.UseCorrelationId();
        app.UseStaticFiles("/files");
        app.UseRouting();
        app.UseVportalRequestLogging();
        app.UseCors();

        app.UseCheckConfigurationMiddleware();
        app.UseEleonsoftHealthChecksMiddleware();

        app.UseMiddleware<EleonsoftMultiTenancyMiddleware>();
        app.UseAuthentication();
        app.UseAbpRequestLocalization(opt => opt.AddDefaultTenantRequestCultureProvider());

        app.UseMiddleware<SessionMiddleware>();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint(pathBase + "/swagger/v1/swagger.json", "VPortal API");
            options.SwaggerEndpoint(pathBase + "/swagger/v2/swagger.json", "VPortal API - Simplified");

            options.RoutePrefix = "swagger";

            options.OAuthClientId(cfg["Swagger:OAuth:ClientId"]);
            options.OAuthClientSecret(cfg["Swagger:OAuth:ClientSecret"]);
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseMiddleware<SigV4AuthMiddleware>();

        app.UseConfiguredEndpoints();
    }
    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        StaticServicesAccessor.Initialize(context.ServiceProvider);
    }   

    private void ConfigureApplicationCookie(IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureApplicationCookie(c =>
        {
            c.Cookie.Path = "/";
            c.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            c.SlidingExpiration = false;
        });
    }

    private void ConfigureAntiforgery()
    {
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });
    }

    private void ConfigureHealthChecks(ServiceConfigurationContext context, IConfiguration configuration)
    {
        // Migrated to V2 HealthChecks architecture
        // Old: context.Services.AddEleonsoftHealthChecks(configuration);
        // Register core infrastructure first
        context.Services.AddEleonHealthChecksCore(configuration);
        // Register all health checks
        context.Services.AddHealthChecks()
            .AddEleonHealthChecksAll(configuration);
        // Register EventBus and RabbitMQ checks (if needed, these may need V2 conversion later)
        context.Services.AddEventBusHealthCheck(configuration);
        context.Services.AddRabbitMqHealthCheck(configuration);
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
            options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
            // TODO: Migration note: there is no email-confirmation for Open-Source version
            //options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
        });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "EleonS3:";
        });
    }

    private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
               
            });
        }
    }

    private void ConfigureConventionalControllers()
    {
        Configure<MvcOptions>(options =>
        {
            options.Conventions.Add(new RemoveAbpControllersConventions());
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddEleonsoftAuthentication(configuration);
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("VPortal");
        if (!hostingEnvironment.IsDevelopment())
        {
            //var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
            //dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "VPortal-Protection-Keys");
        }
    }

    private void ConfigureExternalProviders(ServiceConfigurationContext context)
    {

    }

    private void ConfigureLayoutHooks()
    {
        Configure<AbpLayoutHookOptions>(options =>
        {

        });
    }

    private void ConfigureLocalizationOptions()
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<EleonS3HttpApiHostModule>();
        });
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("he", "he", "עִבְרִית"));
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));

        });
    }

    private void ConfigureAuthorization(IServiceCollection services)
    {
        

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("NotDriver", policy => policy.RequireAssertion(ctx => !IsDriver(ctx.User)));
        });

        bool IsDriver(ClaimsPrincipal principal)
            => IsClient(principal, "driver_client");
        bool IsClient(ClaimsPrincipal principal, string clientType)
            => principal.Claims.Any(c => c.Type == OptionalUserClaims.ClientType);
    }

    private void ConfigureAntiforgery(IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.Cookie.Path = "/";
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }



    private void ConfigureTenentResolvement(IConfiguration cfg)
    {
        Configure<AbpTenantResolveOptions>(options =>
        {
            options.AddHostnameTenantResolver();
        });
    }

    private void ConfigureBundling()
    {
    }

    private void ConfigureAuditing()
    {
        Configure<AbpAuditingOptions>(options =>
        {
            //options.IsEnabledForGetRequests = true;
            options.ApplicationName = "EleonS3";
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
            });
        }
    }


    private void ConfigureJobs()
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }

    private void ConfigureRedis(ServiceConfigurationContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("VPortal");
        if (!hostingEnvironment.IsDevelopment())
        {
        }
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddEleonsoftCorsWithHostnamePolicyProvider();
    }

    public void ConfigureExceptionHandling(IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandling(configuration);
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {

        PostConfigure<StaticFileOptions>(options =>
        {
            options.ContentTypeProvider = options.ContentTypeProvider ?? new FileExtensionContentTypeProvider();

            // Use the WebRootFileProvider to serve files from the real folder

            options.FileProvider = options.FileProvider ?? context.Services.GetHostingEnvironment().WebRootFileProvider;

            var filesProvider = new ManifestEmbeddedFileProvider(GetType().Assembly, "wwwroot");
            options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
        });
    }
}
