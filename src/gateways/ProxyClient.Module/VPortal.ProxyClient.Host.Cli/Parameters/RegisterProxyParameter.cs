using Volo.Abp.DependencyInjection;
using VPortal.ProxyClient.Domain.Auth;
using VPortal.ProxyClient.Domain.Shared.Auth;
using VPortal.ProxyClient.Host.Cli.Cli;

namespace VPortal.ProxyClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class RegisterProxyParameter : CliParameter, ITransientDependency
    {
        private static string ErrorRegistrationState = "Error";
        private static Dictionary<string, string> RegistrationStages = new()
        {
            { ProxyClientRegistrationStages.NotRegistered, "Registering..." },
            { ProxyClientRegistrationStages.Registered, "Confirming..." },
            { ProxyClientRegistrationStages.Completed, "Completed" },
            { ErrorRegistrationState, "Error occured" },
        };

        private readonly ProxyAuthDomainService authDomainService;

        public RegisterProxyParameter(ProxyAuthDomainService authDomainService)
        {
            this.authDomainService = authDomainService;
        }

        public override string ShortName => "r";

        public override string LongName => "register";

        public override string Description => "Registers current machine for further usage as a proxy client.";

        public override string[] ValuesDescriptions { get; } = new string[] { "Registration key" };

        public override async Task Execute(CliArgument arg)
        {
            string regKey = arg.Values.First();
            await foreach (var stage in authDomainService.EnsureRegistered(regKey))
            {
                var stageName = RegistrationStages[stage ?? ErrorRegistrationState];
                Console.WriteLine(stageName);
            }
        }
    }
}
