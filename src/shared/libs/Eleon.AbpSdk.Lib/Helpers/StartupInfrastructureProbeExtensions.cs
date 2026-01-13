using Common.Module.Constants;
using Eleon.Logging.Lib.SystemLog.Logger;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net.Sockets;

namespace EleonsoftSdk.modules.Helpers.Module;

public static class StartupInfrastructureProbeExtensions
{
  private const int DefaultRedisTimeoutSeconds = 2;
  private const int DefaultRabbitMqTimeoutSeconds = 3;
  private const int DefaultRedisPort = 6379;
  private const int DefaultRabbitMqPort = 5672;
  private const int MassTransitRabbitMqTransportType = 1;

  public static async Task<IConfiguration> ApplyStartupInfrastructureChecksAsync(this IConfiguration configuration)
  {
    if (configuration == null)
    {
      return configuration;
    }

    configuration = await TryDisableRedisIfUnavailableAsync(configuration);
    await EnsureRabbitMqAvailableAsync(configuration);

    return configuration;
  }

  private static async Task<IConfiguration> TryDisableRedisIfUnavailableAsync(IConfiguration configuration)
  {
    var isEnabled = configuration.GetValue<bool?>("Redis:IsEnabled") ?? true;
    var rawConfig = configuration.GetValue<string>("Redis:Configuration");

    if (!isEnabled || string.IsNullOrWhiteSpace(rawConfig))
    {
      return configuration;
    }

    if (!TryParseHostPortFromConnectionString(rawConfig, DefaultRedisPort, out var host, out var port))
    {
      return configuration;
    }

    var timeoutSeconds = GetTimeoutSeconds(configuration, "Redis:ProbeTimeoutSeconds", DefaultRedisTimeoutSeconds);
    var canConnect = await CanConnectTcpAsync(host, port, TimeSpan.FromSeconds(timeoutSeconds));
    if (canConnect)
    {
      return configuration;
    }

    var warnMessage = $"Redis is not reachable at {host}:{port}. Disabling Redis and continuing with in-memory cache.";
    EleonsoftLog.Warn(warnMessage);
    Log.Warning(warnMessage);

    return BuildConfigurationOverride(configuration, new Dictionary<string, string?>
    {
      ["Redis:IsEnabled"] = "false"
    });
  }

  private static async Task EnsureRabbitMqAvailableAsync(IConfiguration configuration)
  {
    if (!TryGetRabbitMqEndpoint(configuration, out var host, out var port))
    {
      return;
    }

    var timeoutSeconds = GetTimeoutSeconds(configuration, "EventBus:RabbitMqProbeTimeoutSeconds", DefaultRabbitMqTimeoutSeconds);
    timeoutSeconds = GetTimeoutSeconds(configuration, "EventBus:ProbeTimeoutSeconds", timeoutSeconds);

    var canConnect = await CanConnectTcpAsync(host, port, TimeSpan.FromSeconds(timeoutSeconds));
    if (canConnect)
    {
      return;
    }

    var errorMessage = $"RabbitMQ is not reachable at {host}:{port}. EventBus (MassTransit/RabbitMQ) cannot start.";
    EleonsoftLog.Error(errorMessage);
    Log.Error(errorMessage);
    throw new InvalidOperationException(errorMessage);
  }

  private static bool TryGetRabbitMqEndpoint(IConfiguration configuration, out string host, out int port)
  {
    host = string.Empty;
    port = DefaultRabbitMqPort;

    var provider = configuration.GetValue<int?>("EventBus:Provider");
    if (!provider.HasValue || provider.Value != (int)EventBusProvider.MassTransit)
    {
      return false;
    }

    var transportType = configuration.GetValue<int?>("EventBus:ProviderOptionsJson:Type");
    if (transportType.HasValue && transportType.Value != MassTransitRabbitMqTransportType)
    {
      return false;
    }

    host = configuration.GetValue<string>("EventBus:ProviderOptionsJson:RabbitMqOptions:Host") ?? string.Empty;
    if (string.IsNullOrWhiteSpace(host))
    {
      return false;
    }

    var configuredPort = configuration.GetValue<int?>("EventBus:ProviderOptionsJson:RabbitMqOptions:Port");
    if (configuredPort.HasValue && configuredPort.Value > 0)
    {
      port = configuredPort.Value;
    }

    return port > 0;
  }

  private static int GetTimeoutSeconds(IConfiguration configuration, string key, int fallbackSeconds)
  {
    var configured = configuration.GetValue<int?>(key);
    if (configured.HasValue && configured.Value > 0)
    {
      return configured.Value;
    }

    return fallbackSeconds;
  }

  private static async Task<bool> CanConnectTcpAsync(string host, int port, TimeSpan timeout)
  {
    try
    {
      using var client = new TcpClient();
      var connectTask = client.ConnectAsync(host, port);
      var completed = await Task.WhenAny(connectTask, Task.Delay(timeout));
      if (completed != connectTask)
      {
        return false;
      }

      await connectTask;
      return client.Connected;
    }
    catch
    {
      return false;
    }
  }

  private static bool TryParseHostPortFromConnectionString(string connectionString, int defaultPort, out string host, out int port)
  {
    host = string.Empty;
    port = defaultPort;

    if (string.IsNullOrWhiteSpace(connectionString))
    {
      return false;
    }

    var firstPart = connectionString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .FirstOrDefault();

    if (string.IsNullOrWhiteSpace(firstPart))
    {
      return false;
    }

    var hostPort = firstPart.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    host = hostPort[0];
    if (hostPort.Length > 1 && int.TryParse(hostPort[1], out var parsedPort))
    {
      port = parsedPort;
    }

    return !string.IsNullOrWhiteSpace(host);
  }

  private static IConfiguration BuildConfigurationOverride(IConfiguration baseConfig, IDictionary<string, string?> overrides)
  {
    return new ConfigurationBuilder()
        .AddConfiguration(baseConfig)
        .AddInMemoryCollection(overrides!)
        .Build();
  }
}
