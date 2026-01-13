using Eleon.Logging.Lib.SystemLog.Contracts;
using Eleon.Logging.Lib.SystemLog.Enrichers;
using Eleon.Logging.Lib.SystemLog.Extensions;
using Eleon.Logging.Lib.SystemLog.Logger;
using Eleon.Logging.Lib.SystemLog.Sinks;
using EleonsoftModuleCollector.Commons.Module.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharedModule.modules.AppSettings.Module;
using SharedModule.modules.Logging.Module.SystemLog.Sinks;
using SharedModule.modules.Otel.Module;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using VPortal.Migrations;

namespace VPortal;

public class Program
{
    private static string ConfigurationPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    public async static Task<int> Main(string[] args)
    {
        StartupArgsParser.SetArgs(args);
        Console.WriteLine($"Args: {string.Join(", ", args)}");
        Console.WriteLine($"Configuration path: {ConfigurationPath}");

        AbpCommonDbProperties.DbTablePrefix = "Ec";

        IConfiguration configuration = null;
        try
        {
            configuration = SettingsLoader.LoadConfiguration();

            if (configuration.GetValue<bool>("EnableDebug") && !Debugger.IsAttached)
            {
                Debugger.Launch();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Configuration loading failed: {ex.Message}");
        }

        ConfigureLogger(configuration);

        if (configuration != null)
        {
            var connectionString = configuration.GetConnectionString("Default");
            if (!string.IsNullOrEmpty(connectionString))
            {
                await IsDatabaseAvailableAsync(connectionString);
            }
        }

        if ((StartupArgsParser.IsMigrate || StartupArgsParser.IsSeed) && !StartupArgsParser.IsRun)
        {
            return await MigrateApplication(args);
        }
        else
        {
            if (configuration?.GetValue<bool>("Migrate") == true)
            {
                StartupArgsParser.Set("migrate", "true");
            }

            if (configuration?.GetValue<bool>("Seed") == true)
            {
                StartupArgsParser.Set("seed", "true");
            }

            return await RunApplication(args);
        }
    }

    private static WebApplicationBuilder CreateWebAppBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var config = SettingsLoader.LoadConfiguration();
        builder.Configuration.AddConfiguration(config);

        builder.Host
            .UseAutofac()
            .UseEleonsoftLog()
            .AddSerilogProvider()
            //.UseEleonsoftOtel()
            ;

        return builder;
    }

    private async static Task<int> RunApplication(string[] args)
    {
        // App code goes here. Dispose the SDK before exiting to flush events.
        try
        {
            Log.Information("Starting VPortal.EleonS3.HttpApi.Host.");

            var builder = CreateWebAppBuilder(args);
            await builder.AddApplicationAsync<EleonS3HttpApiHostModule>();
            var app = builder.Build();

            if (StartupArgsParser.IsMigrate || StartupArgsParser.IsSeed)
            {
                await app
                    .Services
                    .GetRequiredService<EleonS3MigrationService>()
                    .MigrateAsync();
            }

            await app.InitializeApplicationAsync();

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            EleonsoftLog.Error("Host terminated unexpectedly!", ex);
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
            EleonsoftLog.FlushAndStop();
        }
    }

    private async static Task<int> MigrateApplication(string[] args)
    {
        try
        {
            Log.Information("Starting EleonS3 migration.");

            var builder = CreateWebAppBuilder(args);
            await builder.AddApplicationAsync<EleonS3MigrationModule>();
            var app = builder.Build();

            await app
                .Services
                .GetRequiredService<EleonS3MigrationService>()
                .MigrateAsync();

            return 0;
        }
        catch (Exception ex)
        {
            EleonsoftLog.Error("Migrations terminated unexpectedly!", ex);
            Log.Fatal(ex, "Migrations terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
            EleonsoftLog.FlushAndStop();
        }
    }

    private static void ConfigureLogger(IConfiguration configuration)
    {
        try
        {
            if (configuration == null)
            {
                Log.Warning("Application configuration was empty");
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Logger static configuration failed");
        }

        if (configuration != null && !configuration.GetSection("Serilog").Exists())
        {
            Log.Warning("Serilog configuration was empty");
        }

        Log.Information("Logger was configured from: {cfgPath}", ConfigurationPath);

        try
        {
            configuration.ConfigureEleonsoftLogger(
                    [
                        new ConsoleSystemLogSink().Filter(configuration, "Logger:Sinks:Console"),
                    //new FileSystemLogSink($"Logs/SystemLogs/Log_{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}.txt").Filter(configuration, "Logger:Sinks:File", SystemLogLevel.Warning),
                    new DbSystemLogSink(configuration?.GetConnectionString("Default")).Filter(configuration, "Logger:Sinks:Database", SystemLogLevel.Critical)
                    ],
                    [new HostInfoEnricher()]);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "System log configuration failed");
        }
    }

    private const int MaxRetries = 3;
    private const int DelayMilliseconds = 60000; // 1 minute

    public static async Task<bool> IsDatabaseAvailableAsync(string connectionString)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
        Log.Logger.Information($"Connection string: {connectionString}");
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                return true; // Connection successful
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Database check failed (Attempt {attempt}/{MaxRetries}): {ex.Message}");
                if (attempt < MaxRetries)
                {
                    await Task.Delay(DelayMilliseconds);
                }
            }
        }

        Log.Logger.Error("Database is not available");
        throw new Exception("Database is not available");
    }
}
