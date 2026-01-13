using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Sentry;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VPortal.GatewayClient.HttpApi.Host;
using VPortal.GatewayClient.Domain.Auth;
using System.ComponentModel;
using VPortal.GatewayClient.Host.Exceptions;
using VPortal.GatewayClient.Config;
using VPortal.GatewayClient.Domain.Shared.Constants;
using VPortal.GatewayClient.Host.Cli;
using VPortal.GatewayClient.Host.Cli.Configuration;
using VPortal.GatewayClient.Host.Cli.Helpers;
using System.Diagnostics;
using VPortal.GatewayClient.Host.WssLoopback;
using VPortal.GatewayClient.Host.EventBus;

namespace VPortal.GatewayClient.Host
{
    public class GatewayClientHost
    {
        private static string LoggerLevel = "Info";
        private static string SentryDsn = "Info";
        private static string SentryEnvironment = "";
        private static bool SentryDebug = false;
        private static bool SentryEnable = false;
        private static bool SentryPerformance = false;
        private static Mutex? appRunningMutex;

        public static async Task<int> ConfigureAndRun(string[] args)
        {
            Debugger.Launch();
            Configure();
            
            var startupType = await GatewayClientHostCli.ProcessArguments(args);
            if (startupType == HostStartupType.None)
            {
                if (Debugger.IsAttached)
                {
                    startupType = HostStartupType.Console;
                }
                else
                {
                    return 0;
                }
            }

            try
            {
                CheckLicense();
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLineError($"License is invalid: {ex.Message}");
                return 1;
            }

            if (!IsSingleInstance())
            {
                return 2;
            }

            ConsoleHelper.WriteLineInfo("Initializing host...");
            var host = await GatewayClientHttpApiHost.InitializeHost(args, builder =>
            {
                if (startupType is HostStartupType.Service)
                {
                    ConfigureWindowsService(builder);
                }
            });
            ConsoleHelper.WriteLineSuccess("Host initialized.");

            return await RunHost(host);
        }

        private static bool IsSingleInstance()
        {
            appRunningMutex = new Mutex(false, GatewayClientHostConsts.HostMutexName);
            if (appRunningMutex.WaitOne(0, false))
            {
                return true;
            }

            return false;
        }

        private static void CheckLicense()
        {
            try
            {
                LicenseHelper.ValidateLicense();
            }
            catch (Exception ex)
            {
                var licenseEx = new GatewayHostLicenseException(ex.Message);
                Log.Error(licenseEx, "An error occured on license check");
                throw licenseEx;
            }
        }

        private static void ConfigureWindowsService(IHostBuilder hostBuilder)
        {
            hostBuilder.UseWindowsService(cfg =>
            {
                cfg.ServiceName = GatewayClientHostConsts.HostServiceName;
            });
        }

        private static async Task<int> RunHost(IHost host)
        {
            ConsoleHelper.WriteLineInfo("Running host...");
            using (SentrySdk.Init(o =>
            {
                o.Dsn = SentryEnable ? SentryDsn : "";
                o.Debug = SentryDebug;
                o.Environment = SentryEnvironment;
                o.TracesSampleRate = SentryPerformance ? 1.0 : 0.0;
            }))
            {
                try
                {
                    await host.StartAsync();
                    ConsoleHelper.WriteLineSuccess("Host is up and running.");

                    await host.WaitForShutdownAsync();
                    ConsoleHelper.WriteLineInfo("Host is shut down.");
                    return 0;
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Host terminated unexpectedly!");
                    ConsoleHelper.WriteLineError("Host terminated unexpectedly!");
                    return 1;
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
        }

        private static void Configure()
        {
            using FileStream stream = File.OpenRead(GatewayClientConfigConsts.AppsettingsPath);
            Dictionary<string, object> jsonSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(stream);

            foreach (var item in jsonSettings)
            {
                if (item.Key == "Logger")
                {
                    var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Value.ToString());

                    LoggerLevel = settings.ContainsKey("Level") ? settings["Level"].ToString() : "Info";
                }
                if (item.Key == "Sentry")
                {
                    var settings = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Value.ToString());

                    SentryDsn = settings.ContainsKey("Dsn") ? settings["Dsn"].ToString() : "";
                    SentryEnvironment = settings.ContainsKey("Environment") ? settings["Environment"].ToString() : "";
                    SentryEnable = settings.ContainsKey("Enable") ? Convert.ToBoolean(settings["Enable"].ToString()) : false;
                    SentryDebug = settings.ContainsKey("Debug") ? Convert.ToBoolean(settings["Debug"].ToString()) : false;
                    SentryPerformance = settings.ContainsKey("Performance") ? Convert.ToBoolean(settings["Performance"].ToString()) : false;
                }
            }

            var logger = new LoggerConfiguration();

            if (LoggerLevel == "Info")
            {
                logger.MinimumLevel.Information();
                logger.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information);
            }
            else if (LoggerLevel == "Debug")
            {
                logger.MinimumLevel.Debug();
                logger.MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug);
            }
            else if (LoggerLevel == "Warning")
            {
                logger.MinimumLevel.Warning();
                logger.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
            }
            else if (LoggerLevel == "Error")
            {
                logger.MinimumLevel.Error();
                logger.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error);
            }
            else
            {
                logger.MinimumLevel.Fatal();
                logger.MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Fatal);
            }

            Log.Logger = logger.Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(GatewayClientConfigConsts.HostLogsPath, "Log.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
