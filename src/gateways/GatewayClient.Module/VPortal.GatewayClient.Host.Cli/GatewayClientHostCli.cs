using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayClient.Domain.Status;
using VPortal.GatewayClient.Host.Cli.Configuration;
using VPortal.GatewayClient.Host.Cli.Helpers;

namespace VPortal.GatewayClient.Host.Cli
{
    public class GatewayClientHostCli
    {
        public static async Task<HostStartupType> ProcessArguments(string[] args)
        {
            HostStartupType hostStartupType = HostStartupType.None;

            using (var application = await AbpApplicationFactory.CreateAsync<GatewayClientHostCliModule>(options =>
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.AddJsonFile($"appsettings.{environmentName}.json", true, true);
                builder.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true);
                options.Services.ReplaceConfiguration(builder.Build());
                options.UseAutofac();
            }))
            {
                await application.InitializeAsync();
                
                var processor = application.ServiceProvider.GetRequiredService<GatewayClientHostCliProcessor>();
                await processor.ProcessArguments(args);

                var startupCfg = application.ServiceProvider.GetRequiredService<HostStartupConfiguration>();
                hostStartupType = startupCfg.HostStartupType;

                var statusService = application.ServiceProvider.GetRequiredService<GatewayClientStatusDomainService>();
                var status = await statusService.GetGatewayStatus();
                ConsoleHelper.WriteLineHighlight("\nGateway Client status (-h for help):\n" + status);
                
                await application.ShutdownAsync();
            }

            return hostStartupType;
        }
    }
}
