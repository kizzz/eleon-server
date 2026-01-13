
using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp;
using Volo.Abp.VirtualFileSystem;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.Caching;
using Volo.Abp.Localization;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using VPortal.Identity.Module;
using VPortal.ProxyClient.Host.Collector;
using VPortal.ProxyClient.HttpApi.Host.AdminSafelist;
using Authorization.Module;
using Authorization.Module.MachineKeyValidation;
using Eleon.Logging.Lib.VportalLogging;

namespace VPortal;

[DependsOn(
    typeof(ProxyClientHostCollector),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(IdentityHttpApiClientModule),
    typeof(AuthorizationModule)
    )]
public class ProxyClientHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        var hostingEnvironment = context.Services.GetHostingEnvironment();

        context.Services.AddVportalLogging(configuration);

        ConfigureMachineKeyValidation();
        ConfigureAntiforgery();
        ConfigureConventionalControllers();
        ConfigureAuthentication(context, configuration);
        ConfigureSwagger(context, configuration);
        ConfigureLocalizationOptions();
        ConfigureCache(configuration);
        ConfigureVirtualFileSystem(context);
        ConfigureDataProtection(context, configuration, hostingEnvironment);
        ConfigureCors(context, configuration);
        ConfigureCompression(context);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseVportalRequestLogging();
        app.UseCors();
        app.UseMiddleware<HostSafelistMiddleware>(configuration["App:HostSafelist"]);
        app.UseAuthentication();

        app.UseAuthorization();
        app.UseMachineKeyValidation();

        app.UseResponseCompression();

        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "VPortal Proxy API");
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
    }

    private void ConfigureCompression(ServiceConfigurationContext context)
    {
        context.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
    }

    private void ConfigureMachineKeyValidation()
    {
        Configure<MachineKeyValidationOptions>(options =>
        {
            options.RequireMachineKey = true;
        });
    }

    private void ConfigureAntiforgery()
    {
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = true;
        });
    }

    private void ConfigureLocalizationOptions()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية", "ae"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish", "fi"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français", "fr"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский", "ru"));
            options.Languages.Add(new LanguageInfo("sl", "sl", "Slovenščina"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak", "sk"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
        });
    }

    private void ConfigureCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "VPortal:";
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
        //Configure<AbpAspNetCoreMvcOptions>(options =>
        //{
        //    options.ConventionalControllers.Create(typeof(VPortalApplicationModule).Assembly);
        //});
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "VPortal";
            });
    }

    private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"],
            new Dictionary<string, string>
            {
                    {"VPortal", "VPortal API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "VPortal API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            });
    }

    private void ConfigureDataProtection(
        ServiceConfigurationContext context,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        //var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName("VPortal");
        //if (!hostingEnvironment.IsDevelopment())
        //{
        //    var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
        //    dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "VPortal-Protection-Keys");
        //}
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        //context.Services.AddCors(options =>
        //{
        //    options.AddDefaultPolicy(builder =>
        //    {
        //        builder
        //            .WithOrigins(
        //                configuration["App:CorsOrigins"]
        //                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
        //                    .Select(o => o.Trim().RemovePostFix("/"))
        //                    .ToArray()
        //            )
        //            .WithAbpExposedHeaders()
        //            .SetIsOriginAllowedToAllowWildcardSubdomains()
        //            .AllowAnyHeader()
        //            .AllowAnyMethod()
        //            .AllowCredentials();
        //    });
        //});
    }
}
