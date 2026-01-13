using Volo.Abp.DependencyInjection;
using VPortal.GatewayClient.Domain.Auth;
using VPortal.GatewayClient.Domain.Shared.Auth;
using VPortal.GatewayClient.Host.Cli.Cli;

namespace VPortal.GatewayClient.Host.Cli.Parameters
{
    [ExposeServices(typeof(CliParameter))]
    public class RegisterGatewayParameter : CliParameter, ITransientDependency
    {
        private static string ErrorRegistrationState = "Error";
        private static Dictionary<string, string> RegistrationStages = new()
        {
            { GatewayClientRegistrationStages.NotRegistered, "Registering..." },
            { GatewayClientRegistrationStages.Registered, "Confirming..." },
            { GatewayClientRegistrationStages.Completed, "Completed" },
            { GatewayClientRegistrationStages.Pending, "Registration request generated. Repeat after server confirmation." },
            { ErrorRegistrationState, "Error occured" },
        };

        private readonly GatewayAuthDomainService authDomainService;

        public RegisterGatewayParameter(GatewayAuthDomainService authDomainService)
        {
            this.authDomainService = authDomainService;
        }

        public override string ShortName => "r";

        public override string LongName => "register";

        public override string Description => "Registers current machine for further usage as a gateway client.";

        public override string[] ValuesDescriptions { get; } = new string[] { "Registration key" };

        public override async Task Execute(CliArgument arg)
        {
            string regKey = arg.Values.First();
            await foreach (var stage in authDomainService.EnsureRegistered(regKey))
            {
                var stageText = RegistrationStages[stage ?? ErrorRegistrationState];
                Console.WriteLine(stageText);

                if (stage == GatewayClientRegistrationStages.Pending)
                {
                    return;
                }
            }
        }
    }
}
