using Eleoncore.SDK;
using Eleoncore.SDK.Helpers;
using EleonsoftSdk.Overrides;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using VPortal.SdkAuthorization;

namespace EleonsoftSdk.Auth;
public static class AuthExtensions
{
  public static IServiceCollection AddEleonsoftAuthentication(this IServiceCollection services, IConfiguration configuration)
  {
    var dataProtectionBuilder = services.AddDataProtection()
        .SetApplicationName(configuration.GetValue("ApplicationName", "EleonsoftService"));


    services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
          {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            // ValidateActor = false,

            ValidateIssuerSigningKey = false,
            SignatureValidator = (token, parameters) =>
                {
                  return EleonsoftAuthTokenValidationHelper.ValidateSignature(token, parameters);
                },
            IssuerValidator = (iss, token, opt) =>
                {
                  return EleonsoftAuthTokenValidationHelper.ValidateIssuer(iss);
                },
          };

          // Configuration for the SignalR (https://learn.microsoft.com/en-us/aspnet/core/signalr/srv/coren-and-authz?view=aspnetcore-7.0#built-in-jwt-authentication)
          options.Events = new JwtBearerEvents
          {
            OnAuthenticationFailed = context =>
                {
                  var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<EleonsoftAuthTokenValidationHelper>>();
                  logger.LogError(context.Exception, "Authentication failed.");
                  return Task.CompletedTask;
                },
            OnTokenValidated = context =>
                {
                  var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<EleonsoftAuthTokenValidationHelper>>();
                  logger.LogDebug("Token validated successfully.");
                  return Task.CompletedTask;
                },
            OnChallenge = context =>
                {
                  var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<EleonsoftAuthTokenValidationHelper>>();
                  logger.LogDebug("JWT challenge triggered.");
                  return Task.CompletedTask;
                },
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
          options.MapInboundClaims = false;
        })
        .AddCertificate(options =>
        {
          options.AllowedCertificateTypes = Microsoft.AspNetCore.Authentication.Certificate.CertificateTypes.All;
          options.Events.OnCertificateValidated = context =>
              {
              return Task.CompletedTask;
            };
        });

    var authority = configuration["App:Authority"];
    if (!string.IsNullOrEmpty(authority))
    {
      EleonsoftAuthTokenValidationHelper.Authorities.Add(authority);
    }

    var authorities = configuration.GetSection("App:Authorities").Get<string[]>() ?? [];

    foreach (var auth in authorities)
    {
      if (!string.IsNullOrEmpty(auth) && !EleonsoftAuthTokenValidationHelper.Authorities.Contains(auth))
      {
        EleonsoftAuthTokenValidationHelper.Authorities.Add(auth);
      }
    }

    services.AddTransient<IClaimsTransformation, ServiceClaimTransformation>();
    return services;
  }
}
