using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks;
using Serilog.Events;
using System.Threading.Tasks;
using System.IO;

namespace SharedModule.modules.Helpers.Module;
public static class ConfigureDefaultLoggerHelper
{
  public static void ConfigureDefaultSerilog(IConfiguration configuration)
  {
    try
    {
      if (configuration == null)
      {
        // Don't use Log.Warning here - Serilog isn't configured yet
        // Use Console or Debug output instead
        System.Diagnostics.Debug.WriteLine("Application configuration was empty");
        return;
      }

      // Create a minimal logger first to avoid blocking on ReadFrom.Configuration
      // If ReadFrom.Configuration fails or hangs, we at least have a working logger
      LoggerConfiguration loggerConfig;

      // Try to read from configuration with a timeout to prevent blocking
      try
      {
        var configTask = Task.Run(() =>
        {
          return new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .Enrich.FromLogContext();
        });

        // Wait up to 5 seconds for configuration read
        if (configTask.Wait(TimeSpan.FromSeconds(5)))
        {
          loggerConfig = configTask.Result;
        }
        else
        {
          // Timeout - use minimal configuration
          System.Diagnostics.Debug.WriteLine("Serilog configuration read timed out, using defaults");
          loggerConfig = new LoggerConfiguration()
              .MinimumLevel.Information()
              .Enrich.FromLogContext();
        }
      }
      catch (Exception configEx)
      {
        // If ReadFrom.Configuration fails, use minimal configuration
        // Don't block startup - log to console instead
        System.Diagnostics.Debug.WriteLine($"Serilog configuration read failed, using defaults: {configEx.Message}");
        loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext();
      }

      try
      {
        var logsRoot = Path.Combine(AppContext.BaseDirectory, "Logs");
        var allDir = Path.Combine(logsRoot, "All");
        var errorDir = Path.Combine(logsRoot, "Error");

        Directory.CreateDirectory(allDir);
        Directory.CreateDirectory(errorDir);

        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var allPath = Path.Combine(allDir, $"serilog-{stamp}.log");
        var errorPath = Path.Combine(errorDir, $"serilog-{stamp}.log");

        if (!File.Exists(allPath)) File.WriteAllText(allPath, string.Empty);
        if (!File.Exists(errorPath)) File.WriteAllText(errorPath, string.Empty);

        loggerConfig = loggerConfig
            .WriteTo.File(allPath, shared: true)
            .WriteTo.File(errorPath, restrictedToMinimumLevel: LogEventLevel.Error, shared: true);
      }
      catch
      {
        // best effort only
      }

      Log.Logger = loggerConfig.CreateLogger();
    }
    catch (Exception ex)
    {
      // Create a minimal fallback logger if everything fails
      try
      {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .CreateLogger();
        Log.Logger?.Error(ex, "Logger static configuration failed, using minimal console logger");
      }
      catch
      {
        // Last resort - use Debug output
        System.Diagnostics.Debug.WriteLine($"Logger configuration failed: {ex.Message}");
      }
    }

    // Only log warning if logger is now configured
    if (Log.Logger != null && configuration != null && !configuration.GetSection("Serilog").Exists())
    {
      try
      {
        Log.Warning("Serilog configuration was empty");
      }
      catch
      {
        // Silently ignore if logging fails
      }
    }
  }

  

  public static void ConfigureSentry(IConfiguration configuration)
  {
    bool isSentryConfigInited = false;

    string sentryDsn = "";
    string sentryEnvironment = "";
    bool sentryDebug = false;
    bool sentryEnable = true;
    bool sentryPerformance = false;

    try
    {
      if (configuration != null)
      {
        sentryDsn = configuration["Sentry:Dsn"] ?? "";
        sentryDebug = configuration.GetValue<bool>("Sentry:Debug");
        sentryEnable = configuration.GetValue<bool>("Sentry:Enable", true);
        sentryPerformance = configuration.GetValue<bool>("Sentry:Performance");
        sentryEnvironment = configuration["Sentry:Environment"] ?? "";

        isSentryConfigInited = !string.IsNullOrEmpty(sentryDsn);
      }
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Sentry configuration failed");
    }

    if (isSentryConfigInited && sentryEnable)
    {
      try
      {
        SentrySdk.Init(o =>
        {
          o.Dsn = sentryDsn;
          // When configuring for the first time, to see what the SDK is doing:
          o.Debug = sentryDebug;
          o.Environment = sentryEnvironment;
          // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
          // We recommend adjusting this value in production.
          o.TracesSampleRate = sentryPerformance ? 1.0 : 0.0;
        });
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Sentry initialization failed");
      }
    }
  }
}
