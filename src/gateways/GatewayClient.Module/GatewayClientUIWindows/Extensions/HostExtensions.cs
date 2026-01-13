using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using System.Threading;
using System.IO;
using VPortal.GatewayClient.Config;

namespace VPortal.GatewayClient.UI.Windows.Extensions
{
    public static class HostExtensions
    {
        public static IHostBuilder UseLogger(this IHostBuilder builder)
        {
            string logsPath = GatewayClientConfigConsts.UiLogsDirectoryPath;
            Directory.CreateDirectory(logsPath);
            var cfg = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(logsPath, "Log.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            return builder.UseSerilog(cfg);
        }

        public static IHostBuilder UseConfiguration(this IHostBuilder builder)
        {
            return builder.ConfigureHostConfiguration(cfg =>
            {
                cfg.SetBasePath(AppContext.BaseDirectory);
                cfg.AddJsonFile("appsettings.json", optional: false);
                cfg.AddJsonFile("appsettings.secrets.json", optional: false);
            });
        }

        internal static IHost InitializeApplication([NotNull] this IHost host)
        {
            Check.NotNull(host, nameof(host));

            var application = host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>();
            var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                application.Shutdown();
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                application.Dispose();
            });

            application.Initialize(host.Services);

            return host;
        }

        public static async Task InitializeAndStart([NotNull] this IHost host, CancellationToken cancellationToken)
        {
            host.InitializeApplication();
            await host.StartAsync(cancellationToken);
        }
    }
}
