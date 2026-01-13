using Eleon.Logging.Lib.SystemLog.Extensions;
using Eleon.Logging.Lib.SystemLog.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migrations.Module;
using Serilog;
using SharedModule.modules.AppSettings.Module;
using SharedModule.modules.Helpers.Module;
using SharedModule.modules.Otel.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal;

namespace EleonsoftSdk.modules.Helpers.Module;
public class EleoncoreWebApplication
{
  public static async Task<WebApplication> CreateWebApplicationAsync<TStartupModule>(string[] args, IConfiguration configuration, Func<WebApplicationBuilder, Task>? configure = null) where TStartupModule : IAbpModule
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration.Sources.Clear();
    builder.Configuration.AddConfiguration(configuration);

    builder.Host
        .UseAutofac()
        .UseEleonsoftLog()
        .AddSerilogProvider()
        .UseEleonsoftOtel()
        ;

    if (configure != null)
    {
      await configure.Invoke(builder);
    }

    await builder.AddApplicationAsync<TStartupModule>();

    return builder.Build();
  }

  public static async Task<int> HostWebApplicationAsync<TStartupModule>(string[] args, Func<WebApplicationBuilder, Task>? configureBuilder = null, Func<WebApplication, Task>? configureApp = null) where TStartupModule : IAbpModule
  {
    try
    {
      StartupArgsParser.SetArgs(args);

      var configuration = SettingsLoader.LoadConfiguration(StartupArgsParser.ConfigurationProfile);

      DebugHelper.AttachDebuggerIfNeeded(configuration);
      ConfigureDefaultLoggerHelper.ConfigureDefaultSerilog(configuration);
      configuration.ConfigureDefaultEleoncoreLogger();
      ConfigureDefaultLoggerHelper.ConfigureSentry(configuration);

      configuration = await configuration.ApplyStartupInfrastructureChecksAsync();

      Log.Information($"Starting {typeof(TStartupModule).Name} host.");

      await DatabaseCheckHelper.CheckDefaultDbConnection(configuration);

      var app = await CreateWebApplicationAsync<TStartupModule>(StartupArgsParser.Raw, configuration, configureBuilder);

      if (configureApp != null)
      {
        await configureApp.Invoke(app);
      }

      var shouldRun = StartupArgsParser.IsRun || !(StartupArgsParser.IsMigrate || StartupArgsParser.IsSeed);

      if (configuration?.GetValue<bool>("Migrate") == true)
      {
        StartupArgsParser.Set("migrate", "true");
      }

      if (configuration?.GetValue<bool>("Seed") == true)
      {
        StartupArgsParser.Set("seed", "true");
      }

      var result = 0;
      if (StartupArgsParser.IsMigrate || StartupArgsParser.IsSeed)
      {
        try
        {
          var migrationService = app.Services.GetService<IDbMigrationService>();
          if (migrationService != null)
          {
            await migrationService.MigrateAsync();
          }
          else
          {
            EleonsoftLog.Warn("No migration service found to perform database migration/seed.");
          }
        }
        catch (Exception ex)
        {
          result = 1;
          EleonsoftLog.Error($"Database migration/seed failed! {ex.Message}", ex);
        }
      }

      if (shouldRun)
      {
        await app.InitializeApplicationAsync();

        await app.RunAsync();
      }

      return result;
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "Host terminated unexpectedly!");
      EleonsoftLog.Error("Host terminated unexpectedly!", ex);
      return 1;
    }
    finally
    {
      Log.CloseAndFlush();
      EleonsoftLog.FlushAndStop();
    }
  }
}
