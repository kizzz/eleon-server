using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.GatewayClient.Domain.Auth;
using VPortal.GatewayClient.Domain.Settings;
using VPortal.GatewayClient.Domain.Shared.Auth;
using VPortal.GatewayClient.Host.Cli.Cli;
using VPortal.GatewayClient.Host.Cli.Configuration;
using VPortal.GatewayClient.Host.Cli.Helpers;

namespace VPortal.GatewayClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class HelpParameter : CliParameter, ITransientDependency
    {
        private readonly IServiceProvider serviceProvider;

        public override string ShortName => "h";

        public override string LongName => "help";

        public override string Description => "Prints help listing all VPortal Gateway Client CLI commands.";

        public override string[] ValuesDescriptions { get; } = new string[] { };

        public HelpParameter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override async Task Execute(CliArgument arg)
        {
            var parameters = serviceProvider.GetRequiredService<IEnumerable<CliParameter>>();

            var builder = new StringBuilder();
            builder.AppendLine("VPortal Gateway Client CLI is used to configure and/or start gateway host.\n");
            foreach (var param in parameters)
            {
                builder.Append($"-{param.ShortName}, /{param.ShortName}, --{param.LongName} ");
                foreach (var val in param.ValuesDescriptions)
                {
                    builder.Append($"[{val}] ");
                }
                builder.AppendLine($"\t {param.Description}");
            }

            ConsoleHelper.WriteLineInfo(builder.ToString());
        }
    }
}
