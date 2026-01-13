using Eleon.Logging.Lib.SystemLog.Logger;
using Eleoncore.Module.ContentSecurity;
using Eleoncore.Module.TenantHostname;
using Eleoncore.SDK.Helpers;
using Eleoncore.SDK.MicroserviceInitialization;
using Eleoncore.SDK.TenantSettings;
using EleoncoreProxy.Client;
using EleonsoftAbp.Auth;
using EleonsoftProxy.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SharedModule.HttpApi.Helpers;
using TenantSettings.Module.Cache;
using Volo.Abp.Authorization;
using Volo.Abp.Security.Claims;

namespace Eleoncore.SDK.Extensions
{
  public static class EleoncoreServiceCollectionExtensions
  {

    public static IServiceCollection ConfigureEleoncoreProxy(this IServiceCollection services, Action<EleoncoreSdkConfig> config, ILoggerFactory? loggerFactory = null, bool addApiToDi = true)
    {
      var myConfig = new EleoncoreSdkConfig();
      config.Invoke(myConfig);
      ;
      ApiConfigurator.Initialize(nameof(EleoncoreProxy), new ApiConfigurator(myConfig, loggerFactory ?? EleonsoftLoggerFactory.Instance, EleoncoreProxy.Client.HostConfiguration.ConfigureApiJsonOptions()));

      if (addApiToDi)
      {
        services.AddEleoncoreProxyApi(); // register api into DI
        services.AddHttpContextInitializator((sp) => () => sp.GetService<ICurrentPrincipalAccessor>()?.Principal?.GetUserId());
      }

      return services;
    }

    public static IServiceCollection ConfigureEleonsoftProxy(this IServiceCollection services, Action<EleoncoreSdkConfig> config, ILoggerFactory? loggerFactory = null, bool addApiToDi = true)
    {
      var myConfig = new EleoncoreSdkConfig();
      config.Invoke(myConfig);
      ;
      ApiConfigurator.Initialize(nameof(EleonsoftProxy), new ApiConfigurator(myConfig, loggerFactory ?? EleonsoftLoggerFactory.Instance, EleonsoftProxy.Client.HostConfiguration.ConfigureApiJsonOptions()));

      if (addApiToDi)
      {
        services.AddEleonsoftProxyApi(); // register api into DI
        services.AddHttpContextInitializator((sp) => () => sp.GetService<ICurrentPrincipalAccessor>()?.Principal?.GetUserId());
      }

      return services;
    }

    public static IServiceCollection AddEleoncoreSdk(this IServiceCollection services)
    {
      services.AddSingleton<EleoncoreSdkTenantCacheService>();
      services.AddSingleton<EleoncoreTenantConnectionStringService>();
      services.AddSingleton<EleoncoreSdkTenantSettingService>();
      services.AddTransient<EleoncoreSdkTokenForwardingMiddleware>();
      services.AddTransient<IAuthorizationHandler, EleoncoreSdkPermissionRequirementHandler>();
      services.AddScoped<EleoncoreMicroserviceInitializer>();

      //services.AddEleoncoreSdkAuthorization();

      return services;
    }
    public static IServiceCollection AddEleoncoreSdkAuthentication(this IServiceCollection services)
    {
      var dataProtectionBuilder = services.AddDataProtection().SetApplicationName("VPortal");


      services
          .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
              ValidateIssuerSigningKey = false,
              SignatureValidator = (token, parameters) =>
                    {
                    return SignatureValidatorHelper.ValidateSignature(token, parameters);
                  },
              IssuerValidator = (iss, token, opt) =>
                    {
                    return EleoncoreIssuerValidatorHelper.ValidateIssuer(iss);
                  },
            };

            // Configuration for the SignalR (https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-7.0#built-in-jwt-authentication)
            options.Events = new JwtBearerEvents
            {
              OnMessageReceived = context =>
                    {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    var tokenProvidedInQuery = !string.IsNullOrEmpty(accessToken);
                    var isHubRequest = path.StartsWithSegments("/hubs");

                    if (isHubRequest && tokenProvidedInQuery)
                    {
                      context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                  }
            };

            options.RequireHttpsMetadata = true;
            options.Audience = "VPortal";
            options.MapInboundClaims = true;
          })
          .AddCertificate(options =>
          {
            options.AllowedCertificateTypes = Microsoft.AspNetCore.Authentication.Certificate.CertificateTypes.All;
            options.Events.OnCertificateValidated = context =>
                  {
                return Task.CompletedTask;
              };
          });

      return services.Replace(ServiceDescriptor.Transient<IAuthorizationPolicyProvider, EleoncoreSdkPolicyProvider>());
    }
    public static IServiceCollection AddEleoncoreSdkClientIsolation(this IServiceCollection services)
    {
      return services
          //.AddTransient<EleoncoreClientIsolationMiddleware>()
          //.AddTransient<EleoncoreClientIsolationValidator>()
          ;
    }
    public static IServiceCollection AddEleoncoreContentSecurity(this IServiceCollection services)
    {
      return services.AddTransient<EleoncoreContentSecurityMiddleware>();
    }
    public static IServiceCollection AddEleoncoreMachineKeyValidation(this IServiceCollection services)
    {
      return services
          //.AddTransient<EleoncoreMachineKeyValidationMiddleware>()
          ;
    }
    public static IApplicationBuilder UseEleoncoreSdkTokenForwarding(this IApplicationBuilder app)
    {
      return app.UseMiddleware<EleoncoreSdkTokenForwardingMiddleware>();
    }

    public static IServiceCollection AddEleoncoreCorsWithHostnamePolicyProvider(this IServiceCollection services, Action<CorsPolicyBuilder> cfg = null)
    {
      if (services == null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      if (cfg != null)
      {
        services.AddSingleton<IEleoncoreCorsPolicyConfigurator>(new EleoncoreCorsPolicyConfigurator(cfg));
      }

      services.TryAdd(ServiceDescriptor.Transient<ICorsService, CorsService>());
      services.Replace(ServiceDescriptor.Transient<ICorsPolicyProvider, EleoncoreSdkTenantSettingService>());

      return services;
    }
  }
}
