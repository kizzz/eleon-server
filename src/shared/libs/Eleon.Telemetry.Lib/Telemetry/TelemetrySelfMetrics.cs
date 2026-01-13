using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading;

namespace SharedModule.modules.Otel.Module;

/// <summary>
/// Singleton class that emits self-observability metrics for the telemetry library.
/// Provides counters for configure/export attempts and failures, and gauges for status.
/// </summary>
public sealed class TelemetrySelfMetrics : IDisposable
{
  private readonly Meter _meter;
  private readonly Counter<long> _configureAttemptsCounter;
  private readonly Counter<long> _configureFailuresCounter;
  private readonly Counter<long> _exportFailuresCounter;
  private readonly ObservableGauge<long> _lastSuccessTimestampGauge;
  private readonly ObservableGauge<long> _circuitOpenGauge;
  private readonly ObservableGauge<long> _enabledGauge;

  private long _lastSuccessTimestamp;
  private readonly Dictionary<string, long> _circuitOpenStates = new(); // signal -> 0/1
  private readonly Dictionary<string, long> _enabledStates = new(); // signal -> 0/1
  private readonly object _lock = new();

  public TelemetrySelfMetrics()
  {
    _meter = new Meter("Eleon.Telemetry");

    _configureAttemptsCounter = _meter.CreateCounter<long>(
      "telemetry_configure_attempts_total",
      description: "Total number of telemetry configuration attempts");

    _configureFailuresCounter = _meter.CreateCounter<long>(
      "telemetry_configure_failures_total",
      description: "Total number of telemetry configuration failures");

    _exportFailuresCounter = _meter.CreateCounter<long>(
      "telemetry_export_failures_total",
      description: "Total number of telemetry export failures",
      unit: "{failures}");

    _lastSuccessTimestampGauge = _meter.CreateObservableGauge<long>(
      "telemetry_last_success_timestamp",
      description: "Unix timestamp (seconds) of last successful telemetry configuration",
      unit: "s",
      observeValue: () => Interlocked.Read(ref _lastSuccessTimestamp));

    _circuitOpenGauge = _meter.CreateObservableGauge<long>(
      "telemetry_circuit_open",
      unit: "{state}",
      description: "Circuit breaker state per signal (0=closed, 1=open)",
      observeValues: () =>
      {
        lock (_lock)
        {
          return _circuitOpenStates.Select(kvp => new Measurement<long>(kvp.Value, new KeyValuePair<string, object?>("signal", kvp.Key)));
        }
      });

    _enabledGauge = _meter.CreateObservableGauge<long>(
      "telemetry_enabled",
      unit: "{state}",
      description: "Telemetry enabled state per signal (0=disabled, 1=enabled)",
      observeValues: () =>
      {
        lock (_lock)
        {
          return _enabledStates.Select(kvp => new Measurement<long>(kvp.Value, new KeyValuePair<string, object?>("signal", kvp.Key)));
        }
      });
  }

  public void IncrementConfigureAttempts()
  {
    _configureAttemptsCounter.Add(1);
  }

  public void IncrementConfigureFailures()
  {
    _configureFailuresCounter.Add(1);
  }

  public void RecordExportFailure(string signal)
  {
    _exportFailuresCounter.Add(1, new KeyValuePair<string, object?>("signal", signal));
  }

  public void SetLastSuccessTimestamp(long unixSeconds)
  {
    Interlocked.Exchange(ref _lastSuccessTimestamp, unixSeconds);
  }

  public void SetCircuitOpenState(string signal, bool isOpen)
  {
    lock (_lock)
    {
      _circuitOpenStates[signal] = isOpen ? 1L : 0L;
    }
  }

  public void SetEnabledState(string signal, bool isEnabled)
  {
    lock (_lock)
    {
      _enabledStates[signal] = isEnabled ? 1L : 0L;
    }
  }

  public void Dispose()
  {
    _meter?.Dispose();
  }
}
