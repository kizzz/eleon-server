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

namespace VPortal.GatewayClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class ChangePortParameter : CliParameter, ITransientDependency
    {
        private readonly GatewayClientSettingsDomainService settingsDomainService;

        public override string ShortName => "p";

        public override string LongName => "port";

        public override string Description => "Changes host port to the one provided.";

        public override string[] ValuesDescriptions { get; } = new string[] { "New port value" };

        public ChangePortParameter(GatewayClientSettingsDomainService settingsDomainService)
        {
            this.settingsDomainService = settingsDomainService;
        }

        public override async Task Execute(CliArgument arg)
        {
            string portStr = arg.Values.First();
            
            if (!int.TryParse(portStr, out var port))
            {
                throw new ArgumentException("Port value should be an integer between 0 and 65535.");
            }

            if (port is < 0 or > 65535)
            {
                throw new ArgumentException("Port value should be an integer between 0 and 65535.");
            }

            await settingsDomainService.SetPort(port);
        }
    }
}
