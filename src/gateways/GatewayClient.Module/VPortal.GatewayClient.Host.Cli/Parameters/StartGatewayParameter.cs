using Volo.Abp.DependencyInjection;
using VPortal.GatewayClient.Host.Cli.Cli;
using VPortal.GatewayClient.Host.Cli.Configuration;

namespace VPortal.GatewayClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class StartGatewayParameter : CliParameter, ITransientDependency
    {
        private readonly HostStartupConfiguration hostStartConfiguration;

        public StartGatewayParameter(HostStartupConfiguration hostStartConfiguration)
        {
            this.hostStartConfiguration = hostStartConfiguration;
        }

        public override string ShortName => "s";

        public override string LongName => "start";

        public override string Description => "Starts host up.";

        public override string[] ValuesDescriptions { get; } = new string[] { "Startup type (console / service)" };

        public override async Task Execute(CliArgument arg)
        {
            string type = arg.Values.First();
            
            if (string.Equals(type, nameof(HostStartupType.Console), StringComparison.OrdinalIgnoreCase))
            {
                hostStartConfiguration.HostStartupType = HostStartupType.Console;
            }
            else if (string.Equals(type, nameof(HostStartupType.Service), StringComparison.OrdinalIgnoreCase))
            {
                hostStartConfiguration.HostStartupType = HostStartupType.Service;
            }
        }
    }
}
