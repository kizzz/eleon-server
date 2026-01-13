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
using VPortal.ProxyClient.Domain.Status;
using VPortal.ProxyClient.Host.Cli.Configuration;
using VPortal.ProxyClient.Host.Cli.Helpers;

namespace VPortal.ProxyClient.Host.Cli
{
    public class ProxyClientHostCli
    {
        public static async Task<HostStartupType> ProcessArguments(string[] args)
        {
            HostStartupType hostStartupType = HostStartupType.None;

            using (var application = await AbpApplicationFactory.CreateAsync<ProxyClientHostCliModule>(options =>
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
                
                var processor = application.ServiceProvider.GetRequiredService<ProxyClientHostCliProcessor>();
                await processor.ProcessArguments(args);

                var startupCfg = application.ServiceProvider.GetRequiredService<HostStartupConfiguration>();
                hostStartupType = startupCfg.HostStartupType;

                var statusService = application.ServiceProvider.GetRequiredService<ProxyClientStatusDomainService>();
                var status = await statusService.GetProxyStatus();
                ConsoleHelper.WriteLineHighlight("\nProxy Client status (-h for help):\n" + status);
                
                await application.ShutdownAsync();
            }

            return hostStartupType;
        }
    }
}
