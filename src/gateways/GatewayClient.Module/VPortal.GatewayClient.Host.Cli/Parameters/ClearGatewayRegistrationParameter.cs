using Volo.Abp.DependencyInjection;
using VPortal.GatewayClient.Domain.Auth;
using VPortal.GatewayClient.Host.Cli.Cli;

namespace VPortal.GatewayClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class ClearGatewayRegistrationParameter : CliParameter, ITransientDependency
    {
        private readonly GatewayAuthDomainService authDomainService;

        public ClearGatewayRegistrationParameter(GatewayAuthDomainService authDomainService)
        {
            this.authDomainService = authDomainService;
        }

        public override string ShortName => "c";

        public override string LongName => "clear-registration";

        public override string Description => "Clears the host registration info.";

        public override string[] ValuesDescriptions { get; } = new string[] {};

        public override async Task Execute(CliArgument arg)
        {
            await authDomainService.ResetRegistration();
        }
    }
}
