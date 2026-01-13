using abp_sdk.Middlewares;
using AutoMapper;
using Common.Module.Constants;
using Eleon.Common.Lib.Helpers;
using Eleon.Common.Lib.UserToken;
using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Logging.Lib.VportalLogging;
using Eleon.Startup.Lib;
using Eleoncore.Abp.Sdk;
using Eleoncore.SDK.Extensions;
using Eleoncore.SDK.RequestLocalization;
using EleoncoreAspNetCoreSdk.HealthChecks.Overrides;
using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.Implementations.CheckConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using EleonsoftSdk.modules.Helpers.Module;
using Logging.Module;
using Logging.Module.ErrorHandling.Extensions;
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
using Microsoft.OpenApi.Models;
using Migrations.Module;
using StackExchange.Redis;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
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
    typeof(AbpAspNetCoreSignalRModule),
    typeof(EleoncoreAbpSdkModule)
    )]
public class EleoncoreHostModule : AbpModule
{
  #region Services Configuration

  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    // disable storing permissions and features in its database
    Configure<PermissionManagementOptions>(options =>
    {
      options.IsDynamicPermissionStoreEnabled = false;
      options.SaveStaticPermissionsToDatabase = false;
    });
    Configure<FeatureManagementOptions>(options =>
    {
      options.IsDynamicFeatureStoreEnabled = false;
      options.SaveStaticFeaturesToDatabase = false;
    });
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

    var configuration = context.Services.GetConfiguration();
    var hostingEnvironment = context.Services.GetHostingEnvironment();
    var applicationName = configuration.GetValue("ApplicationName", "Undefined");

    //context.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    //{
    //    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    //});
    //context.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
    //{
    //    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    //});
    context.Services
        .AddControllers(options =>
        {
          options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
          //options.Conventions.Add(new GlobalRoutePrefixConvention(new RouteAttribute("core")));

        });

    Configure<AbpAspNetCoreMvcOptions>(options =>
    {
      // this makes ABP expose controllers from this assembly as endpoints
      options.ConventionalControllers.Create(typeof(EleoncoreHostModule).Assembly);
    });

    //.AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    //.AddNewtonsoftJson(opt =>
    //{
    //    opt.SerializerSettings.ContractResolver = new DefaultContractResolver();
    //    opt.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    //});

    Configure<AbpUnitOfWorkDefaultOptions>(options =>
    {
      options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
    });

    Configure<AbpMvcLibsOptions>(options =>
    {
      options.CheckLibs = false;
    });

    Configure<MvcOptions>(options =>
    {
      options.Conventions.Add(new RemoveAbpControllersConventions());
    });

    Configure<AbpClockOptions>(options =>
    {
      options.Kind = DateTimeKind.Utc; // set UTC datetime to the application (ex. CreationTime will be in UTC)
    });

    context.Services.AddVportalLogging(configuration);

    context.Services.AddAutoMapper(typeof(EleoncoreHostModule));
    context.Services.AddMemoryCache();
    context.Services.AddHttpContextAccessor();
    context.Services.AddCurrentUserToken();


    Configure<AbpAntiForgeryOptions>(options =>
    {
      options.AutoValidate = false;
    });
    context.Services.AddAntiforgery(options =>
    {
      options.Cookie.Path = "/";
      options.Cookie.SameSite = SameSiteMode.None;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    Configure<AbpAuditingOptions>(options =>
    {
      //options.IsEnabledForGetRequests = true;
      options.ApplicationName = applicationName;
    });

    Configure<AbpDistributedCacheOptions>(options =>
    {
      options.KeyPrefix = $"{applicationName}:";
    });

    Configure<AbpBackgroundJobOptions>(options =>
    {
      options.IsJobExecutionEnabled = false;
    });

    context.Services.AddAuthorization(opt =>
    {
      opt.AddPolicy("NotDriver", policy => policy.RequireAssertion(ctx => !ctx.User.Claims.Any(c => c.Type == OptionalUserClaims.ClientType && c.Value == "driver_client")));
    });

    ConfigureSwagger(context, configuration, applicationName);
    ConfigureApplicationCookie(context.Services, configuration);
    ConfigureMiniProfiler(context, configuration);
    ConfigureVirtualFileSystem(context);
    ConfigureLocalizationOptions();
    ConfigureUrls(configuration);
    ConfigureExternalProviders(context);
    ConfigureDataProtection(context, configuration, hostingEnvironment, applicationName);

    context.Services.AddHttpClient();
    context.Services.AddExceptionHandling(configuration);
    
    // Migrated to V2 HealthChecks architecture
    // Old: context.Services.AddCommonHealthChecks(configuration);
    // Register core infrastructure first
    context.Services.AddEleonHealthChecksCore(configuration);
    // Register all health checks
    context.Services.AddHealthChecks()
        .AddEleonHealthChecksAll(configuration);

    context.Services.ConfigureEleoncoreProxy(o =>
    {
      o.BaseHost = configuration.GetValue("EleoncoreSdk:BaseHost", string.Empty);
      o.BasePath = configuration.GetValue("EleoncoreSdk:BasePath", string.Empty);
      o.OAuthUrl = configuration.GetValue("EleoncoreSdk:OAuthUrl", string.Empty);
      o.UseOAuthAuthorization = configuration.GetValue("EleoncoreSdk:UseOAuthAuthorization", false);
      o.ApiAuthUrl = configuration.GetValue("EleoncoreSdk:ApiAuthUrl", string.Empty);
      o.ApiKey = configuration.GetValue("EleoncoreSdk:ApiKey", string.Empty);
      o.UseApiAuthorization = configuration.GetValue("EleoncoreSdk:UseApiAuthorization", false);
      o.ApiKeySecret = configuration.GetValue("EleoncoreSdk:ApiKeySecret", string.Empty);
      o.ClientSecret = configuration.GetValue("EleoncoreSdk:SecretKey", string.Empty);
      o.ClientId = configuration.GetValue("EleoncoreSdk:AppKey", string.Empty);
      o.IgnoreSslValidation = configuration.GetValue("EleoncoreSdk:IgnoreSslValidation", false);
    });

    context.Services.ConfigureEleonsoftProxy(o =>
    {
      o.BaseHost = configuration.GetValue("EleoncoreSdk:BaseEleonsoftHost", string.Empty);
      o.BasePath = configuration.GetValue("EleoncoreSdk:BasePath", string.Empty);
      o.OAuthUrl = configuration.GetValue("EleoncoreSdk:OAuthUrl", string.Empty);
      o.UseOAuthAuthorization = configuration.GetValue("EleoncoreSdk:UseOAuthAuthorization", false);
      o.ApiAuthUrl = configuration.GetValue("EleoncoreSdk:ApiAuthUrl", string.Empty);
      o.ApiKey = configuration.GetValue("EleoncoreSdk:ApiKey", string.Empty);
      o.UseApiAuthorization = configuration.GetValue("EleoncoreSdk:UseApiAuthorization", false);
      o.ApiKeySecret = configuration.GetValue("EleoncoreSdk:ApiKeySecret", string.Empty);
      o.ClientSecret = configuration.GetValue("EleoncoreSdk:SecretKey", string.Empty);
      o.ClientId = configuration.GetValue("EleoncoreSdk:AppKey", string.Empty);
      o.IgnoreSslValidation = configuration.GetValue("EleoncoreSdk:IgnoreSslValidation", false);
    });

    context.Services.AddEleoncoreSdk();

    context.Services.AddEleoncoreSdkAuthentication();

    context.Services.AddEleoncoreMultiTenancy(configuration);
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

    context.Services.UseSdkHealthChecks();
  }

  #endregion

  #region Application Initialization

  public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
  {
    StaticServicesAccessor.Initialize(context.ServiceProvider);

    var env = context.ServiceProvider.GetService<IWebHostEnvironment>();
    if (env == null)
    {
      return;
    }

    var cfg = context.GetConfiguration();
    var appName = cfg.GetValue("ApplicationName", "Undefined");

    IApplicationBuilder? app = null;
    try
    {
      app = context.GetApplicationBuilder();
    }
    catch (ArgumentNullException)
    {
      // No application builder in test context.
      return;
    }

    if (env.IsDevelopment() || true)
    {
      try
      {
        // check AutoMapper configuration // it will throw an exception if something is wrong with mappings
        var mapper = context.ServiceProvider.GetRequiredService<IMapper>();
      }
      catch (Exception ex)
      {
        EleonsoftLog.Error("AutoMapper configuration validation failed.", ex);
        throw;
      }
    }

    app.UseExceptionHandlingMiddleware();
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseMultiPathBase(cfg);

    var options = new ForwardedHeadersOptions
    {
      ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
    };
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    app.UseForwardedHeaders(options);

    app.UseCheckConfigurationMiddleware();
    app.UseEleonsoftHealthChecksMiddleware();

    app.UseCorrelationId();
    app.UseUiHosting(cfg);
    app.UseRouting();
    app.UseCors();

    //app.UseMiddleware<NonAuthorizedTenantResolveMiddleware>();
    //app.UseMiddleware<NonAuthorizedClientIsolationMiddleware>();
  }

  public override void OnApplicationInitialization(ApplicationInitializationContext context)
  {
    var env = context.ServiceProvider.GetService<IWebHostEnvironment>();
    if (env == null)
    {
      return;
    }

    var cfg = context.GetConfiguration();
    IApplicationBuilder? app = null;
    try
    {
      app = context.GetApplicationBuilder();
    }
    catch (ArgumentNullException)
    {
      // No application builder in test context.
      return;
    }

    app.UseEleoncoreMultiTenancy();
    app.UseCurrentUserToken();
    app.UseAuthentication();

    // app.UseJwtTokenMiddleware(IdentityConstants.ApplicationScheme);
    //app.UseMiddleware<ClientIsolationMiddleware>();
    //app.UseMiddleware<AntiforgeryValidationMiddleware>();

    app.UseAuthorization();
    //app.UseContentSecurity();

    app.UseAuditing();
    app.UseAbpSerilogEnrichers();
    app.UseUnitOfWork();


    app.UseAbpRequestLocalization(opt => opt.AddDefaultEleoncoreTenantRequestCultureProvider());
    app.UseEleoncoreSdkTokenForwarding();

    app.UseRouting();
    app.UseVportalRequestLogging();
  }

  public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
  {
    var env = context.ServiceProvider.GetService<IWebHostEnvironment>();
    if (env == null)
    {
      return;
    }

    var cfg = context.GetConfiguration();
    var appName = cfg.GetValue("ApplicationName", "Undefined");
    var pathBase = cfg.GetValue("App:PathBase", string.Empty);
    IApplicationBuilder? app = null;
    try
    {
      app = context.GetApplicationBuilder();
    }
    catch (ArgumentNullException)
    {
      // No application builder in test context.
      return;
    }

    app.UseMiniProfiler();
    app.UseSwagger();
    app.UseAbpSwaggerUI(options =>
    {
      options.SwaggerEndpoint(pathBase + "/swagger/v1/swagger.json", $"{appName} API");
      options.SwaggerEndpoint(pathBase + "/swagger/v2/swagger.json", $"{appName} API - Simplified");

      options.RoutePrefix = "swagger";

      options.OAuthClientId(cfg["Swagger:OAuth:ClientId"]);
      options.OAuthClientSecret(cfg["Swagger:OAuth:ClientSecret"]);
    });

    app.UseConfiguredEndpoints();
  }

  #endregion

  #region private helpers

  private static void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration, string appName)
  {
    var authority = configuration["App:Authority"];
    if (string.IsNullOrWhiteSpace(authority))
    {
      return;
    }

    context.Services.AddAbpSwaggerGenWithOAuth(
        authority,
        new Dictionary<string, string> { { "VPortal", "VPortal API" } },
        options =>
        {
          options.SwaggerDoc("v1", new OpenApiInfo { Title = $"{appName} API", Version = "v1" });
          options.DocInclusionPredicate((docName, description) => true);
          options.CustomSchemaIds(GetName);
          options.DescribeAllParametersInCamelCase();
          options.SchemaFilter<EnumSchemaFilter>();
          options.DocumentFilter<IncludeClassesForSdkSwaggerFilter>();

          //options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
          //{
          //    Type = SecuritySchemeType.ApiKey,
          //    Name = VPortalExtensionGrantsConsts.ApiKey.ApiKeyParameter,
          //    In = ParameterLocation.Query, // or Header depending on your API
          //    Description = "Provide the API Key as a query parameter for authentication."
          //});

          //options.AddSecurityRequirement(new OpenApiSecurityRequirement
          //{
          //    {
          //        new OpenApiSecurityScheme
          //        {
          //            Reference = new OpenApiReference
          //            {
          //                Type = ReferenceType.SecurityScheme,
          //                Id = "ApiKey"
          //            }
          //        },
          //        Array.Empty<string>() // No specific scopes
          //    }
          //});

          options.SwaggerDoc("v2", new OpenApiInfo { Title = $"{appName} API - Simplified", Version = "v2" });
          options.DocInclusionPredicate((docName, description) => true);
          options.CustomOperationIds(t =>
          {
            TypeInfo controllerTypeInfo = null;
            if (t.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
              controllerTypeInfo = controllerActionDescriptor.ControllerTypeInfo;
              // Example: Set operation ID as {ControllerName}_{ActionName}_{HttpMethod}
            }
            var namespaceName = controllerTypeInfo != null ?
                    controllerTypeInfo.Namespace
                        .Replace("VPortal.", "")
                        .Replace("Volo.Abp.", "")
                        .Split(".")
                        .FirstOrDefault() : "";
            var actionName = t.ActionDescriptor.RouteValues["action"];
            var controllerName = t.ActionDescriptor.RouteValues["controller"];
            var httpMethod = t.HttpMethod;

            // api/roles  | GET, POST


            // Customize operation ID format as needed
            return $"{namespaceName}{controllerName}{actionName}";
          });
          options.CustomSchemaIds(GetName);
          options.DescribeAllParametersInCamelCase();
          //options.SchemaFilter<PropertySchemaFilter>();

          static string GetName(System.Type type)
          {
            if (type.IsGenericType)
            {
              var genericArgNames = type.GetGenericArguments().Select(GetName);
              return GetNameForEleoncore(type) + "Of" + genericArgNames.JoinAsString("And");
            }

            return GetNameForEleoncore(type);
          }

          static string GetNameForEleoncore(System.Type t)
          {
            var fullName = GetFullNameWithoutGenericArity(t)
                    .ReplaceFirst("Volo.Abp.", "Eleoncore.")
                    .ReplaceFirst("CoreInfrastructure.", "Eleoncore.")
                    .ReplaceFirst("Infrastructure.", "Eleoncore.")
                    .ReplaceFirst("Common.", "Eleoncore.")
                    .ReplaceFirst("Identity.", "EleoncoreIdentity.")
                    .ReplaceFirst("System.", "Eleoncore.")
                    .ReplaceFirst("VPortal.", "", StringComparison.InvariantCultureIgnoreCase);

            var parts = fullName.Split('.');
            if (parts.Length == 1)
            {
              return parts.First();
            }

            return $"{parts.First()}.{parts.Last()}";
          }

          static string GetNameWithoutGenericArity(System.Type t)
          {
            string name = t.Name;
            int index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
          }

          static string GetFullNameWithoutGenericArity(System.Type t)
          {
            return $"{t.Namespace}.{GetNameWithoutGenericArity(t)}";
          }
        });
  }

  private void ConfigureApplicationCookie(IServiceCollection services, IConfiguration configuration)
  {
    services.ConfigureApplicationCookie(c =>
    {
      c.Cookie.Path = "/";
      c.ExpireTimeSpan = TimeSpan.FromMinutes(60);
      c.SlidingExpiration = false;

      //if (configuration["DebugSettings:IgnoreSslValidation"] == "true")
      //{
      //    c.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // keeps Secure in HTTPS only
      //    c.Cookie.SameSite = SameSiteMode.Lax;
      //}
    });
  }

  private void ConfigureMiniProfiler(ServiceConfigurationContext context, IConfiguration configuration)
  {
    bool enableProfiler = configuration.GetValue("MiniProfiler:Enable", false);
    if (!enableProfiler)
    {
      return;
    }

    context.Services
        .AddMiniProfiler(options =>
        {
          //More options see https://miniprofiler.com/dotnet/AspDotNetCore
        })
        .AddEntityFramework();

    //context.Services.Configure<AbpLayoutHookOptions>(options =>
    //{
    //    options.Add(LayoutHooks.Body.Last, typeof(MiniProfilerViewComponent));
    //});
  }

  private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
  {
    var hostingEnvironment = context.Services.GetHostingEnvironment();

    if (hostingEnvironment.IsDevelopment())
    {
      Configure<AbpVirtualFileSystemOptions>(options =>
      {
        //options.FileSets.ReplaceEmbeddedByPhysical<VPortalDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, "..", "VPortal.Domain.Shared"));
        //options.FileSets.ReplaceEmbeddedByPhysical<VPortalDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, "..", "VPortal.Domain"));
        //options.FileSets.ReplaceEmbeddedByPhysical<VPortalApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, "..", "VPortal.Application.Contracts"));
        //options.FileSets.ReplaceEmbeddedByPhysical<VPortalApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, "..", "VPortal.Application"));
        //options.FileSets.ReplaceEmbeddedByPhysical<VPortalHttpApiModule>(Path.Combine(hostingEnvironment.ContentRootPath, "..", "VPortal.HttpApi"));
      });
    }
  }

  private void ConfigureDataProtection(
      ServiceConfigurationContext context,
      IConfiguration configuration,
      IWebHostEnvironment hostingEnvironment,
      string appName)
  {
    var dataProtectionBuilder = context.Services.AddDataProtection().SetApplicationName(appName);
    var redisEnabled = configuration.GetValue<bool?>("Redis:IsEnabled") ?? true;
    if (!hostingEnvironment.IsDevelopment() && redisEnabled)
    {
      var redisConfig = configuration["Redis:Configuration"];
      if (!string.IsNullOrWhiteSpace(redisConfig))
      {
        try
        {
          var redis = ConnectionMultiplexer.Connect(redisConfig);
          dataProtectionBuilder.PersistKeysToStackExchangeRedis(redis, "VPortal-Protection-Keys");
        }
        catch (Exception ex)
        {
          EleonsoftLog.Warn("Redis is not available for DataProtection. Falling back to in-memory keys.", ex);
        }
      }
      else
      {
        EleonsoftLog.Warn("Redis configuration is missing for DataProtection. Falling back to in-memory keys.");
      }
    }
  }

  private void ConfigureLocalizationOptions()
  {
    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<EleoncoreHostModule>();
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



      // ERP resources

      //var erpExternalLocalizationResources = ExternalLocalizationHelper.GetChildDirectoryNames("C:\\Workspace\\eleoncore\\server\\hosts\\core-unified\\ExternalLocalization\\Eleonsphere");
      //foreach (var erpResource in erpExternalLocalizationResources)
      //{
      //    options.Resources.Add(erpResource, "en")
      //    .AddBaseTypes(typeof(AbpValidationResource))
      //    .AddVirtualJson($"/ExternalLocalization/Eleonsphere/{erpResource}");
      //}
      // Immunities resources
      //var immuExternalLocalizationResources = ExternalLocalizationHelper.GetChildDirectoryNames("C:\\Workspace\\eleoncore\\server\\hosts\\core-unified\\ExternalLocalization\\Immunities");

      //foreach (var immuResource in immuExternalLocalizationResources)
      //{
      //    options.Resources.Add(immuResource, "en")
      //    .AddBaseTypes(typeof(AbpValidationResource))
      //    .AddVirtualJson($"/ExternalLocalization/Immunities/{immuResource}");
      //}
    });
  }

  private void ConfigureUrls(IConfiguration configuration)
  {
    Configure<AppUrlOptions>(options =>
    {
      options.Applications["Angular"].RootUrl = configuration["App:AngularUrl"];
      options.Applications["Angular"].Urls["Abp.Account.PasswordReset" /*AccountUrlNames.PasswordReset*/ ] = "account/reset-password";
      // TODO: Migration note: there is no email-confirmation for Open-Source version
      //options.Applications["Angular"].Urls[AccountUrlNames.EmailConfirmation] = "account/email-confirmation";
    });
  }

  private void ConfigureExternalProviders(ServiceConfigurationContext context)
  {
    //context.Services
    //.AddDynamicExternalLoginProviderOptions<GoogleOptions>(
    //    GoogleDefaults.AuthenticationScheme,
    //    options =>
    //    {
    //        options.WithProperty(x => x.ClientId);
    //        options.WithProperty(x => x.ClientSecret, isSecret: true);
    //    }
    //)
    //.AddDynamicExternalLoginProviderOptions<MicrosoftAccountOptions>(
    //    MicrosoftAccountDefaults.AuthenticationScheme,
    //    options =>
    //    {
    //        options.WithProperty(x => x.ClientId);
    //        options.WithProperty(x => x.ClientSecret, isSecret: true);
    //    }
    //)
    //.AddDynamicExternalLoginProviderOptions<TwitterOptions>(
    //    TwitterDefaults.AuthenticationScheme,
    //    options =>
    //    {
    //        options.WithProperty(x => x.ConsumerKey);
    //        options.WithProperty(x => x.ConsumerSecret, isSecret: true);
    //    }
    //);
  }

  #endregion
}
