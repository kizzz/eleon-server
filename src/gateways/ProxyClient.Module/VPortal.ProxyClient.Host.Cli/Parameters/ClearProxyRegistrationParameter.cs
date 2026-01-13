using Volo.Abp.DependencyInjection;
using VPortal.ProxyClient.Domain.Auth;
using VPortal.ProxyClient.Host.Cli.Cli;

namespace VPortal.ProxyClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class ClearProxyRegistrationParameter : CliParameter, ITransientDependency
    {
        private readonly ProxyAuthDomainService authDomainService;

        public ClearProxyRegistrationParameter(ProxyAuthDomainService authDomainService)
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
