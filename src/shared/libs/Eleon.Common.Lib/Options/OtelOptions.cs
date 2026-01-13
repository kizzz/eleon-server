using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.modules.Otel.Module;

public class OtelOptions : IEquatable<OtelOptions>
{
  // General settings
  public bool Enabled { get; set; } = false;
  public string ServiceName { get; set; }
  public string? ServiceDescription { get; set; }

  // Telemetry-specific configurations
  public TracesOptions Traces { get; set; } = new();
  public MetricsOptions Metrics { get; set; } = new();
  public LogsOptions Logs { get; set; } = new();

  // Reliability options (global defaults, can be overridden per-signal)
  public ReliabilityOptions? Reliability { get; set; }

  public class TracesOptions
  {
    public string Endpoint { get; set; } = "http://localhost:4318/v1/traces";
    public string Protocol { get; set; } = "http"; // "grpc" | "http"
    public bool UseBatch { get; set; } = true;

    // Instrumentation options
    public bool UseAspNetCoreInstrumentation { get; set; } = true;
    public bool UseHttpClientInstrumentation { get; set; } = true;
    public bool UseSqlClientInstrumentation { get; set; } = true;
    public bool UseMassTransitInstrumentation { get; set; } = true;

    // Optional per-signal reliability override
    public ReliabilityOptions? Reliability { get; set; }
  }

  public class MetricsOptions
  {
    public string Endpoint { get; set; } = "http://localhost:4318/v1/metrics";
    public string Protocol { get; set; } = "http"; // "grpc" | "http"
    public bool UseBatch { get; set; } = true;

    // Instrumentation options
    public bool UseRuntimeInstrumentation { get; set; } = true;
    public bool UseProcessInstrumentation { get; set; } = true;
    public bool UseAspNetCoreInstrumentation { get; set; } = true;
    public bool UseHttpClientInstrumentation { get; set; } = true;

    // Optional per-signal reliability override
    public ReliabilityOptions? Reliability { get; set; }
  }

  public class LogsOptions
  {
    public string Endpoint { get; set; } = "http://localhost:4318/v1/logs";
    public string Protocol { get; set; } = "http"; // "grpc" | "http"
    public bool UseBatch { get; set; } = true;

    // Additional logging options
    public bool IncludeScopes { get; set; } = true;
    public bool IncludeFormattedMessage { get; set; } = true;

    // Optional per-signal reliability override
    public ReliabilityOptions? Reliability { get; set; }
  }

  public class ReliabilityOptions : IEquatable<ReliabilityOptions>
  {
    public HeadersOptions? Headers { get; set; }
    public TlsOptions? Tls { get; set; }
    public CompressionOptions? Compression { get; set; }
    public RetryOptions? Retry { get; set; }
    public CircuitBreakerOptions? CircuitBreaker { get; set; }
    public FallbackOptions? Fallback { get; set; }

    public class HeadersOptions : IEquatable<HeadersOptions>
    {
      // Support both Dictionary and raw OTLP header string format
      public Dictionary<string, string>? Dictionary { get; set; }
      public string? Raw { get; set; } // Format: "k1=v1,k2=v2"

      public bool Equals(HeadersOptions? other)
      {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Compare dictionary presence and keys only (never values for security)
        var thisKeys = Dictionary?.Keys.OrderBy(k => k).ToArray() ?? Array.Empty<string>();
        var otherKeys = other.Dictionary?.Keys.OrderBy(k => k).ToArray() ?? Array.Empty<string>();
        if (!thisKeys.SequenceEqual(otherKeys)) return false;

        // Compare raw string presence only (not content)
        return (Raw == null) == (other.Raw == null);
      }

      public override bool Equals(object? obj) => Equals(obj as HeadersOptions);

      public override int GetHashCode()
      {
        var hash = new HashCode();
        if (Dictionary != null)
        {
          foreach (var key in Dictionary.Keys.OrderBy(k => k))
          {
            hash.Add(key);
          }
        }
        hash.Add(Raw != null);
        return hash.ToHashCode();
      }
    }

    public class TlsOptions : IEquatable<TlsOptions>
    {
      public TlsMode Mode { get; set; } = TlsMode.System;
      public string[]? PinnedThumbprints { get; set; }
      public ClientCertificateOptions? ClientCertificate { get; set; }
      public bool AllowInvalidCertificate { get; set; } = false;

      public enum TlsMode
      {
        System,
        PinnedThumbprint,
        CustomCaBundle
      }

      public class ClientCertificateOptions : IEquatable<ClientCertificateOptions>
      {
        public string? Path { get; set; }
        public string? SecretKey { get; set; }

        public bool Equals(ClientCertificateOptions? other)
        {
          if (other == null) return false;
          if (ReferenceEquals(this, other)) return true;
          // Compare path/key presence only (never actual values)
          return Path == other.Path && SecretKey == other.SecretKey;
        }

        public override bool Equals(object? obj) => Equals(obj as ClientCertificateOptions);

        public override int GetHashCode() => HashCode.Combine(Path != null, SecretKey != null);
      }

      public bool Equals(TlsOptions? other)
      {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;

        if (Mode != other.Mode) return false;
        if (AllowInvalidCertificate != other.AllowInvalidCertificate) return false;

        // Compare thumbprint arrays (presence and length, not values for security)
        var thisThumbs = PinnedThumbprints ?? Array.Empty<string>();
        var otherThumbs = other.PinnedThumbprints ?? Array.Empty<string>();
        if (thisThumbs.Length != otherThumbs.Length) return false;

        return ClientCertificate?.Equals(other.ClientCertificate) ?? other.ClientCertificate == null;
      }

      public override bool Equals(object? obj) => Equals(obj as TlsOptions);

      public override int GetHashCode()
      {
        var hash = new HashCode();
        hash.Add(Mode);
        hash.Add(PinnedThumbprints?.Length ?? 0);
        hash.Add(AllowInvalidCertificate);
        hash.Add(ClientCertificate);
        return hash.ToHashCode();
      }
    }

    public class CompressionOptions : IEquatable<CompressionOptions>
    {
      public bool Enabled { get; set; } = false;
      public CompressionAlgorithm Algorithm { get; set; } = CompressionAlgorithm.Gzip;

      public enum CompressionAlgorithm
      {
        Gzip
      }

      public bool Equals(CompressionOptions? other)
      {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Enabled == other.Enabled && Algorithm == other.Algorithm;
      }

      public override bool Equals(object? obj) => Equals(obj as CompressionOptions);

      public override int GetHashCode() => HashCode.Combine(Enabled, Algorithm);
    }

    public class RetryOptions : IEquatable<RetryOptions>
    {
      public bool Enabled { get; set; } = false;
      public int MaxRetries { get; set; } = 3;
      public int BaseDelayMs { get; set; } = 200;
      public int MaxDelayMs { get; set; } = 2000;
      public int TimeoutMs { get; set; } = 30000;

      public bool Equals(RetryOptions? other)
      {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Enabled == other.Enabled &&
               MaxRetries == other.MaxRetries &&
               BaseDelayMs == other.BaseDelayMs &&
               MaxDelayMs == other.MaxDelayMs &&
               TimeoutMs == other.TimeoutMs;
      }

      public override bool Equals(object? obj) => Equals(obj as RetryOptions);

      public override int GetHashCode() => HashCode.Combine(Enabled, MaxRetries, BaseDelayMs, MaxDelayMs, TimeoutMs);
    }

    public class CircuitBreakerOptions : IEquatable<CircuitBreakerOptions>
    {
      public bool Enabled { get; set; } = false;
      public int FailureThreshold { get; set; } = 5;
      public int SamplingWindowSec { get; set; } = 30;
      public int OpenDurationSec { get; set; } = 60;
      public int HalfOpenMaxAttempts { get; set; } = 1;

      public bool Equals(CircuitBreakerOptions? other)
      {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Enabled == other.Enabled &&
               FailureThreshold == other.FailureThreshold &&
               SamplingWindowSec == other.SamplingWindowSec &&
               OpenDurationSec == other.OpenDurationSec &&
               HalfOpenMaxAttempts == other.HalfOpenMaxAttempts;
      }

      public override bool Equals(object? obj) => Equals(obj as CircuitBreakerOptions);

      public override int GetHashCode() => HashCode.Combine(Enabled, FailureThreshold, SamplingWindowSec, OpenDurationSec, HalfOpenMaxAttempts);
    }

    public class FallbackOptions : IEquatable<FallbackOptions>
    {
      public DevConsoleExporterMode DevConsoleExporter { get; set; } = DevConsoleExporterMode.Off;
      public ProdModeOnFailureMode ProdModeOnFailure { get; set; } = ProdModeOnFailureMode.DisableTelemetry;
      public DisableSignalOnFailureMode DisableSignalOnFailure { get; set; } = DisableSignalOnFailureMode.PerSignal;

      public enum DevConsoleExporterMode
      {
        Off,
        On
      }

      public enum ProdModeOnFailureMode
      {
        DisableTelemetry,
        ConsoleLogsOnly
      }

      public enum DisableSignalOnFailureMode
      {
        PerSignal,
        AllSignals
      }

      public bool Equals(FallbackOptions? other)
      {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return DevConsoleExporter == other.DevConsoleExporter &&
               ProdModeOnFailure == other.ProdModeOnFailure &&
               DisableSignalOnFailure == other.DisableSignalOnFailure;
      }

      public override bool Equals(object? obj) => Equals(obj as FallbackOptions);

      public override int GetHashCode() => HashCode.Combine(DevConsoleExporter, ProdModeOnFailure, DisableSignalOnFailure);
    }

    public bool Equals(ReliabilityOptions? other)
    {
      if (other == null) return false;
      if (ReferenceEquals(this, other)) return true;

      return (Headers?.Equals(other.Headers) ?? other.Headers == null) &&
             (Tls?.Equals(other.Tls) ?? other.Tls == null) &&
             (Compression?.Equals(other.Compression) ?? other.Compression == null) &&
             (Retry?.Equals(other.Retry) ?? other.Retry == null) &&
             (CircuitBreaker?.Equals(other.CircuitBreaker) ?? other.CircuitBreaker == null) &&
             (Fallback?.Equals(other.Fallback) ?? other.Fallback == null);
    }

    public override bool Equals(object? obj) => Equals(obj as ReliabilityOptions);

    public override int GetHashCode()
    {
      var hash = new HashCode();
      hash.Add(Headers);
      hash.Add(Tls);
      hash.Add(Compression);
      hash.Add(Retry);
      hash.Add(CircuitBreaker);
      hash.Add(Fallback);
      return hash.ToHashCode();
    }
  }

  public bool Equals(OtelOptions? other)
  {
    if (other == null) return false;
    if (ReferenceEquals(this, other)) return true;

    return other.Enabled == Enabled &&
           other.ServiceName == ServiceName &&
           other.ServiceDescription == ServiceDescription &&
           other.Traces.Endpoint == Traces.Endpoint &&
           other.Traces.Protocol == Traces.Protocol &&
           other.Traces.UseBatch == Traces.UseBatch &&
           other.Traces.UseAspNetCoreInstrumentation == Traces.UseAspNetCoreInstrumentation &&
           other.Traces.UseHttpClientInstrumentation == Traces.UseHttpClientInstrumentation &&
           other.Traces.UseSqlClientInstrumentation == Traces.UseSqlClientInstrumentation &&
           other.Traces.UseMassTransitInstrumentation == Traces.UseMassTransitInstrumentation &&
           (other.Traces.Reliability?.Equals(Traces.Reliability) ?? Traces.Reliability == null) &&
           other.Metrics.Endpoint == Metrics.Endpoint &&
           other.Metrics.Protocol == Metrics.Protocol &&
           other.Metrics.UseBatch == Metrics.UseBatch &&
           other.Metrics.UseRuntimeInstrumentation == Metrics.UseRuntimeInstrumentation &&
           other.Metrics.UseProcessInstrumentation == Metrics.UseProcessInstrumentation &&
           other.Metrics.UseAspNetCoreInstrumentation == Metrics.UseAspNetCoreInstrumentation &&
           other.Metrics.UseHttpClientInstrumentation == Metrics.UseHttpClientInstrumentation &&
           (other.Metrics.Reliability?.Equals(Metrics.Reliability) ?? Metrics.Reliability == null) &&
           other.Logs.Endpoint == Logs.Endpoint &&
           other.Logs.Protocol == Logs.Protocol &&
           other.Logs.UseBatch == Logs.UseBatch &&
           other.Logs.IncludeScopes == Logs.IncludeScopes &&
           other.Logs.IncludeFormattedMessage == Logs.IncludeFormattedMessage &&
           (other.Logs.Reliability?.Equals(Logs.Reliability) ?? Logs.Reliability == null) &&
           (other.Reliability?.Equals(Reliability) ?? Reliability == null);
  }

  public override bool Equals(object? obj)
  {
    return Equals(obj as OtelOptions);
  }

  public override int GetHashCode()
  {
    var hash = new HashCode();
    hash.Add(Enabled);
    hash.Add(ServiceName);
    hash.Add(ServiceDescription);
    hash.Add(Traces.Endpoint);
    hash.Add(Traces.Protocol);
    hash.Add(Traces.UseBatch);
    hash.Add(Traces.UseAspNetCoreInstrumentation);
    hash.Add(Traces.UseHttpClientInstrumentation);
    hash.Add(Traces.UseSqlClientInstrumentation);
    hash.Add(Traces.UseMassTransitInstrumentation);
    hash.Add(Traces.Reliability);
    hash.Add(Metrics.Endpoint);
    hash.Add(Metrics.Protocol);
    hash.Add(Metrics.UseBatch);
    hash.Add(Metrics.UseRuntimeInstrumentation);
    hash.Add(Metrics.UseProcessInstrumentation);
    hash.Add(Metrics.UseAspNetCoreInstrumentation);
    hash.Add(Metrics.UseHttpClientInstrumentation);
    hash.Add(Metrics.Reliability);
    hash.Add(Logs.Endpoint);
    hash.Add(Logs.Protocol);
    hash.Add(Logs.UseBatch);
    hash.Add(Logs.IncludeScopes);
    hash.Add(Logs.IncludeFormattedMessage);
    hash.Add(Logs.Reliability);
    hash.Add(Reliability);
    return hash.ToHashCode();
  }
}
