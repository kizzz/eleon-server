using Eleoncore.SDK.CoreEvents;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedModule.HttpApi.Helpers;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EleonsoftApiSdk.Services;
public class SdkTelemetrySettingProvider : ITelemetrySettingsProvider, IMessageHandler
{
  private readonly ILogger<SdkTelemetrySettingProvider> _logger;
  private readonly TelemetryConfigurator _telemetryConfigurator;
  private readonly ApiConfigurator _apiConfigurator;
  private readonly object _debounceLock = new();
  private Timer? _debounceTimer;
  private static readonly TimeSpan CooldownPeriod = TimeSpan.FromSeconds(5);
  private static readonly TimeSpan DebounceDelay = TimeSpan.FromMilliseconds(500);
  private DateTime _lastConfigureTime = DateTime.MinValue;

  public SdkTelemetrySettingProvider(
    ILogger<SdkTelemetrySettingProvider> logger,
    TelemetryConfigurator telemetryConfigurator,
    ApiConfigurator? apiConfigurator = null)
  {
    _logger = logger;
    _telemetryConfigurator = telemetryConfigurator;
    _apiConfigurator = apiConfigurator ?? ApiConfigurator.GetConfigurator("EleonsoftProxy");
  }

  public async Task HandleAsync(EventManagementModuleFullEventDto message)
  {
    if (message.Name == "SystemHealthSettingsUpdatedMsg")
    {
      await ScheduleDebouncedConfigureAsync();
    }
  }

  public async Task InitializeAsync()
  {
    await ScheduleDebouncedConfigureAsync();
  }

  private async Task ScheduleDebouncedConfigureAsync()
  {
    lock (_debounceLock)
    {
      // Check cooldown period
      if (DateTime.UtcNow - _lastConfigureTime < CooldownPeriod)
      {
        _logger.LogDebug("Telemetry reconfigure skipped due to cooldown period.");
        return;
      }

      // Cancel existing debounce timer
      _debounceTimer?.Dispose();

      // Schedule new debounced execution
      _debounceTimer = new Timer(async _ => await ExecuteDebouncedConfigureAsync(), null, DebounceDelay, Timeout.InfiniteTimeSpan);
    }

    await Task.CompletedTask;
  }

  private async Task ExecuteDebouncedConfigureAsync()
  {
    lock (_debounceLock)
    {
      _debounceTimer?.Dispose();
      _debounceTimer = null;
    }

    try
    {
      // Use default cancellation token for now
      // The ApiConfigurator should have timeout settings configured
      await ConfigureTelemetryAsync(CancellationToken.None);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error during debounced telemetry configuration.");
    }
  }

  private async Task ConfigureTelemetryAsync(CancellationToken cancellationToken = default)
  {
    var tenantSettingApi = new TenantSettingsApi(_apiConfigurator);

    tenantSettingApi.UseApiAuth();

    // Note: Generated API client methods don't support CancellationToken directly
    // Cancellation is handled at the HttpClient level via ApiConfigurator timeout settings
    var response = await tenantSettingApi.CoreTenantSettingsGetTenantSystemHealthSettingsAsync();

    var settings = response.Ok();

    if (settings == null)
    {
      _logger.LogWarning("Failed to retrieve tenant system health settings during telemetry initialization.");
      return;
    }

    var otel = new OtelOptions
    {
      Enabled = settings.Telemetry?.Enabled ?? false,
      Logs = new OtelOptions.LogsOptions
      {
        Endpoint = settings.Telemetry?.LogsEndpoint ?? string.Empty,
        Protocol = settings.Telemetry?.LogsProtocol ?? string.Empty,
        UseBatch = settings.Telemetry?.LogsUseBatch ?? false,
      },
      Metrics = new OtelOptions.MetricsOptions
      {
        Endpoint = settings.Telemetry?.MetricsEndpoint ?? string.Empty,
        Protocol = settings.Telemetry?.MetricsProtocol ?? string.Empty,
        UseBatch = settings.Telemetry?.MetricsUseBatch ?? false,
      },
      Traces = new OtelOptions.TracesOptions
      {
        Endpoint = settings.Telemetry?.TracesEndpoint ?? string.Empty,
        Protocol = settings.Telemetry?.TracesProtocol ?? string.Empty,
        UseBatch = settings.Telemetry?.TracesUseBatch ?? false,
      },
    };

    await _telemetryConfigurator.ConfigureAsync(otel);
    _lastConfigureTime = DateTime.UtcNow;
  }
}
