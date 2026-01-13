using Eleon.Logging.Lib.VportalLogger;
using Logging.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Eleon.Logging.Lib.VportalLogging;

public static class VportalLoggingExtensions
{
  public static IServiceCollection AddVportalLogging(this IServiceCollection services, IConfiguration configuration, string sectionName = "VportalLogging")
  {
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);

    return services.AddVportalLogging(
        configureExceptions: options => configuration.GetSection($"{sectionName}:Exceptions").Bind(options),
        configureRequestLogging: options => configuration.GetSection($"{sectionName}:Request").Bind(options),
        configureHost: options => configuration.GetSection($"{sectionName}:Host").Bind(options));
  }

  public static IServiceCollection AddVportalLogging(
      this IServiceCollection services,
      Action<VportalExceptionOptions>? configureExceptions = null,
      Action<VportalRequestLoggingOptions>? configureRequestLogging = null,
      Action<VportalLoggingHostOptions>? configureHost = null)
  {
    ArgumentNullException.ThrowIfNull(services);

    services.AddOptions<VportalExceptionOptions>();
    services.AddOptions<VportalRequestLoggingOptions>();
    services.AddOptions<VportalLoggingHostOptions>();

    if (configureExceptions != null)
    {
      services.Configure(configureExceptions);
    }

    if (configureRequestLogging != null)
    {
      services.Configure(configureRequestLogging);
    }

    if (configureHost != null)
    {
      services.Configure(configureHost);
    }

    services.TryAddSingleton<IExceptionClassifier, DefaultExceptionClassifier>();
    services.TryAddSingleton<IExceptionReporter, VportalExceptionReporter>();
    services.TryAddSingleton<IOperationScopeFactory, VportalOperationScopeFactory>();
    services.TryAddSingleton<IBoundaryLogger, BoundaryLogger>();

    var hostOptions = new VportalLoggingHostOptions();
    configureHost?.Invoke(hostOptions);

    if (hostOptions.EnableCompatVportalLogger)
    {
      services.TryAddTransient(typeof(IVportalLogger<>), typeof(VPortalLogger<>));
      services.TryAddTransient(typeof(IVportalControllerLogger<>), typeof(VPortalLogger<>));
    }

    if (hostOptions.EnableSignalRLogging)
    {
      services.TryAddSingleton<IHubFilter, VportalSignalRLoggingFilter>();
      services.Configure<HubOptions>(options => options.AddFilter<VportalSignalRLoggingFilter>());
    }

    return services;
  }

  public static IApplicationBuilder UseVportalRequestLogging(this IApplicationBuilder app)
  {
    ArgumentNullException.ThrowIfNull(app);

    var hostOptions = app.ApplicationServices.GetService<IOptions<VportalLoggingHostOptions>>()?.Value;
    if (hostOptions != null && !hostOptions.EnableHttpRequestLogging)
    {
      return app;
    }

    return app.UseMiddleware<VportalRequestLoggingMiddleware>();
  }
}
