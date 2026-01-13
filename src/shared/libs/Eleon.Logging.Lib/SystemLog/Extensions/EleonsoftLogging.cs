using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Logging.Lib.SystemLog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Reflection.Emit;

namespace Eleon.Logging.Lib.SystemLog.Extensions;

public static class EleonsoftLogging
{
  /// <summary>
  /// Adds Eleonsoft logging to the host builder.
  /// </summary>
  public static IHostBuilder UseEleonsoftLog(
      this IHostBuilder builder,
      Action<EleonsoftLogOptions>? configure = null,
      IEnumerable<ISystemLogSink>? sinks = null,
      IEnumerable<ISystemLogEnricher>? enrichers = null)
  {
    // static pipeline
    var opts = EleonsoftLog.Options ?? new EleonsoftLogOptions();
    configure?.Invoke(opts);
    EleonsoftLog.Configure(opts);

    if (sinks != null)
      foreach (var s in sinks)
        EleonsoftLog.AddSink(s);

    if (enrichers != null)
      foreach (var e in enrichers)
        EleonsoftLog.AddEnricher(e);

    // Ensure at least one sink is configured so logs are visible even if
    // ConfigureDefaultEleonsoftLogger isn't called in the host.
    if (EleonsoftLog.GetSinksSnapshot().Length == 0)
    {
      EleonsoftLog.AddSink(new ConsoleSystemLogSink());
    }

    var provider = new EleonsoftLoggerProvider(opts);

    builder.ConfigureLogging(lb =>
    {
      lb.AddProvider(provider);
    });

    builder.ConfigureServices((_, collection) =>
    {
      // Don't replace ILoggerFactory - let OpenTelemetry and other providers integrate properly
      // Just register the provider as a singleton so it can be accessed if needed
      collection.AddSingleton(provider);
    });

    return builder;
  }

  /// <summary>
  /// Adds Serilog as a parallel logging provider (doesn't replace other providers).
  /// Use this instead of UseSerilog() to keep OpenTelemetry and other providers working.
  /// </summary>
  public static IHostBuilder AddSerilogProvider(
      this IHostBuilder builder,
      Serilog.ILogger? serilogLogger = null,
      bool dispose = false)
  {
    builder.ConfigureLogging((context, loggingBuilder) =>
    {
      // Add Serilog as one provider among many
      var logger = serilogLogger ?? Log.Logger;
      loggingBuilder.AddSerilog(logger, dispose);
    });

    return builder;
  }

  /// <summary>
  /// Configures Eleonsoft logging from configuration. Prefer to use this method for configuring eleonsoft logger
  /// </summary>
  public static void ConfigureEleonsoftLogger(this IConfiguration configuration, IEnumerable<ISystemLogSink>? sinks = null, IEnumerable<ISystemLogEnricher>? enrichers = null, EleonsoftLogOptions? defaultOptions = null, string? configKey = null)
  {
    try
    {
      var options = defaultOptions ?? new EleonsoftLogOptions();

      if (defaultOptions == null)
      {
        var appName = configuration?.GetValue<string>("ApplicationName");

        if (!string.IsNullOrWhiteSpace(appName))
        {
          options.DefaultApplicationName = appName;
        }
      }

      // Overlay config values (only overrides what is set in config)
      configuration?.GetSection(configKey ?? "Logger").Bind(options);

      // Enable forwarding to Serilog if Serilog is already configured
      // This is safe because ConfigureEleonsoftLogger is called after Serilog is initialized
      if (Log.Logger != null && !Log.Logger.GetType().Name.Contains("Silent"))
      {
        options.ForwardToSerilog = true;
      }

      EleonsoftLog.Configure(options);

      if (sinks != null)
      {
        foreach (var sink in sinks)
        {
          EleonsoftLog.AddSink(sink);
        }
      }

      if (enrichers != null)
      {
        foreach (var enricher in enrichers)
        {
          EleonsoftLog.AddEnricher(enricher);
        }
      }

      Log.Information("System log was configured successfully");
    }
    catch (Exception ex)
    {
      Log.Error(ex, "System Log configuration failed");
    }
  }

  public static ISystemLogSink Filter(this ISystemLogSink sink, SystemLogLevel minLogLevel)
      => new FilteringSink(sink, minLogLevel);

  public static ISystemLogSink Filter(this ISystemLogSink sink, IConfiguration configuration, string key, SystemLogLevel fallback = SystemLogLevel.Info)
  {
    var str = configuration?.GetValue<string>(key);
    if (string.IsNullOrWhiteSpace(str) || !Enum.TryParse<SystemLogLevel>(str, true, out var minLogLevel))
    {
      minLogLevel = fallback;
    }

    return new FilteringSink(sink, minLogLevel);
  }

  public static IDictionary<string, string> AddException(this IDictionary<string, string> dict, Exception? exception, string keyBase = "Exception")
  {
    ArgumentNullException.ThrowIfNull(dict);

    if (exception == null)
      return dict;

    if (string.IsNullOrWhiteSpace(keyBase))
      keyBase = "Exception";

    dict[$"{keyBase}_ExceptionType"] = exception.GetType().FullName ?? "Unknown";
    dict[$"{keyBase}_Message"] = exception.Message;
    dict[$"{keyBase}_StackTrace"] = exception.StackTrace ?? string.Empty;
    dict[$"{keyBase}_Source"] = exception.Source ?? string.Empty;
    dict[$"{keyBase}_HResult"] = exception.HResult.ToString();

    foreach (var key in exception.Data.Keys)
    {
      if (key == null) continue;
      dict[$"Exception_Data_{key}"] = exception.Data[key]?.ToString() ?? "null";
    }

    dict.AddException(exception.InnerException, keyBase + "_Inner");

    return dict;
  }

  public static string GenerateHash(SystemLogLevel logLevel, string message, Guid? tenantId, string applicationName)
  {
    // Combine inputs into a consistent string representation
    var combined = $"{(int)logLevel}|{message ?? string.Empty}|{tenantId?.ToString("N") ?? "null"}|{applicationName ?? string.Empty}";

    // Use SHA256 for consistent hashing
    var bytes = System.Text.Encoding.UTF8.GetBytes(combined);
    var hashBytes = System.Security.Cryptography.SHA256.HashData(bytes);

    // Convert to hex string (lowercase for consistency)
    return Convert.ToHexString(hashBytes).ToLowerInvariant();
  }
}
