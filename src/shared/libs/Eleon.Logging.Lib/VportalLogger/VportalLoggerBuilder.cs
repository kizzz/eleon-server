using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Eleon.Logging.Lib.VportalLogger
{
  [Obsolete("Use Serilog host integration: UseSerilog((ctx, services, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)).")]
  public class VportalLoggerBuilder
  {
    private readonly int _fileSizeLimit = 1073741824;
    private readonly RollingInterval _rollingInterval;
    private readonly TimeSpan _retainedFileTimeLimit = TimeSpan.FromDays(7);
    private readonly int _retainedFileCountLimit = 100;

    private readonly List<Action> loggerConfigs = new();
    private readonly List<Func<LogEvent, bool>> filters = new();

    public LoggerConfiguration LoggerConfiguration { get; }

    public VportalLoggerBuilder(LogEventLevel loggingLevel, RollingInterval rollingInterval = RollingInterval.Day, TimeSpan? retainedFileTimeLimit = null, int retainedFileCountLimit = 100)
    {
      _rollingInterval = rollingInterval;
      _retainedFileCountLimit = retainedFileCountLimit;

      if (retainedFileTimeLimit.HasValue)
      {
        _retainedFileTimeLimit = retainedFileTimeLimit.Value;
      }

      LoggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext()
          .MinimumLevel.Is(loggingLevel);

      if (loggingLevel is LogEventLevel.Debug)
      {
        LoggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information);
      }
    }

    public VportalLoggerBuilder(IConfiguration? configuration)
    {
      var loggerLevel = configuration?["Logger:Level"] ?? nameof(LogEventLevel.Information);
      if (!Enum.TryParse<LogEventLevel>(loggerLevel, true, out var logLevel))
      {
        logLevel = LogEventLevel.Information;
      }

      var ri = configuration?["Logger:RollingInterval"] ?? nameof(RollingInterval.Day);
      if (!Enum.TryParse(ri, true, out _rollingInterval))
      {
        _rollingInterval = RollingInterval.Day;
      }

      _fileSizeLimit = configuration?.GetValue("Logger:FileSizeLimitBytes", _fileSizeLimit) ?? _fileSizeLimit;
      _retainedFileCountLimit = configuration?.GetValue("Logger:RetainedFileCountLimit", _retainedFileCountLimit) ?? _retainedFileCountLimit;
      var retainedTimeLimitDays = configuration?.GetValue("Logger:RetainedFileTimeLimitDays", 7) ?? 7;
      _retainedFileTimeLimit = TimeSpan.FromDays(retainedTimeLimitDays);


      LoggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext()
          .MinimumLevel.Is(logLevel);

      if (logLevel is LogEventLevel.Debug)
      {
        LoggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information);
      }
    }


    public VportalLoggerBuilder WriteToFile(
        string path,
        string errorPath = null,
        Func<LogEvent, bool>? filter = null)
    {
      void FileLoggerConfig(LoggerConfiguration l)
      {
        l.WriteTo.Async(c =>
            c.File(
                path,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}]{NewLine}{Message:lj}{NewLine}{Session}{NewLine}{Exception}{NewLine}",
                rollingInterval: _rollingInterval,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: _fileSizeLimit,
                retainedFileCountLimit: _retainedFileCountLimit,
                retainedFileTimeLimit: _retainedFileTimeLimit));

        if (!string.IsNullOrWhiteSpace(errorPath))
        {
          l.WriteTo.Async(c =>
              c.File(
                  errorPath,
                  outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}]{NewLine}{Message:lj}{NewLine}{Session}{NewLine}{Exception}{NewLine}",
                  rollingInterval: _rollingInterval,
                  rollOnFileSizeLimit: true,
                  fileSizeLimitBytes: _fileSizeLimit,
                  restrictedToMinimumLevel: LogEventLevel.Error,
                  retainedFileCountLimit: _retainedFileCountLimit,
                  retainedFileTimeLimit: _retainedFileTimeLimit));
        }
      }

      AddSublogger(FileLoggerConfig, filter);

      return this;
    }

    public ILogger Build()
    {
      foreach (var action in loggerConfigs)
      {
        action();
      }

      return LoggerConfiguration.CreateLogger();
    }

    private void AddSublogger(Action<LoggerConfiguration> configSublogger, Func<LogEvent, bool> filter = null)
    {
      var config = () =>
      {
        LoggerConfiguration.WriteTo.Logger(l =>
              {
            configSublogger(l);

            var exceptedFilters = filters;

            if (filter != null)
            {
              exceptedFilters = exceptedFilters.Except([filter]).ToList();
              l.Filter.ByIncludingOnly(filter);
            }

            foreach (var toExclude in exceptedFilters)
            {
              l.Filter.ByExcluding(toExclude);
            }
          });
      };

      if (filter != null)
      {
        filters.Add(filter);
      }

      loggerConfigs.Add(config);
    }
  }
}
