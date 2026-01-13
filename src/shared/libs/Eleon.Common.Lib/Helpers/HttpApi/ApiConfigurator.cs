using EleonsoftAbp.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedModule.HttpApi.Helpers;
public class ApiConfigurator
{
  public EleoncoreSdkConfig SdkConfig { get; internal set; }
  public ILoggerFactory LoggerFactory { get; internal set; }
  public IHttpContextAccessor? HttpContextAccessor { get; internal set; }
  public JsonSerializerOptions JsonSerializerOptions { get; internal set; }
    public Func<string>? UserIdAccessor { get; internal set; }

    public ApiConfigurator(EleoncoreSdkConfig sdkConfig) : this(sdkConfig, null, null, null)
  {

  }

  public ApiConfigurator(EleoncoreSdkConfig sdkConfig, ILoggerFactory loggerFactory, JsonSerializerOptions jsonSerializerOptions, IHttpContextAccessor? httpContextAccessor = null)
  {
    SdkConfig = sdkConfig;
    LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
    HttpContextAccessor = httpContextAccessor;
    JsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions();
  }

  public void Validate()
  {
    if (SdkConfig == null)
    {
      throw new ApiConfigurationException(nameof(SdkConfig), "SdkConfig cannot be null.");
    }

    if (string.IsNullOrWhiteSpace(SdkConfig.BaseHost))
    {
      throw new ApiConfigurationException(nameof(SdkConfig.BaseHost), "BaseHost cannot be null or empty.");
    }
  }

  public static void Initialize(string proxyName, ApiConfigurator configurator, bool validate = false)
  {
    if (validate)
    {
      configurator.Validate();
    }

    Configurators[proxyName] = configurator;
    configurator.HttpContextAccessor ??= DefaultHttpContextAccessor;
  }

  internal static readonly Dictionary<string, ApiConfigurator> Configurators = new Dictionary<string, ApiConfigurator>();

  public static ApiConfigurator GetConfigurator(string proxyName)
  {
    if (Configurators.TryGetValue(proxyName, out var configurator))
    {
      return configurator;
    }

    throw new ApiConfigurationException($"No configurator found for proxy name: {proxyName}");
  }

  internal static IHttpContextAccessor? DefaultHttpContextAccessor { get; set; } // Accessor captured during service registration // for initializing configurators without direct HttpContextAccessor injection
}

public class ApiConfigurationException : Exception
{
  public ApiConfigurationException(string message)
      : base(message)
  {
  }

  public ApiConfigurationException(string configName, string message)
      : base($"Configuration error in '{configName}': {message}")
  {
  }

  public ApiConfigurationException(string configName, string message, Exception innerException)
      : base($"Configuration error in '{configName}': {message}", innerException)
  {
  }
}

/// <summary>
/// Initializes HttpContextAccessor for ApiConfigurators from the DI container.
/// </summary>
public static class ApiConfiguratorExtensions
{
  private static bool _httpContextInitializatorAdded = false;
  public static IServiceCollection AddHttpContextInitializator(this IServiceCollection services, Func<IServiceProvider, Func<string>>? userIdAccessorProvider = null)
  {
    // Ensure IHttpContextAccessor exists
    services.AddHttpContextAccessor();

    if (_httpContextInitializatorAdded)
      return services;

    services.AddHostedService(sp =>
    {
      var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        var principalAccessor = userIdAccessorProvider?.Invoke(sp);

        ApiConfigurator.DefaultHttpContextAccessor ??= httpContextAccessor;

      foreach (var configurator in ApiConfigurator.Configurators.Values)
      {
        configurator.HttpContextAccessor ??= httpContextAccessor;
            configurator.UserIdAccessor = principalAccessor;
        }
      return new NullHostedService();
    });

    _httpContextInitializatorAdded = true;

    return services;
  }

  private class NullHostedService : IHostedService
  {
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
