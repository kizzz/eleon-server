using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.GatewayClient.Host.Cli.Cli;
using VPortal.GatewayClient.Host.Cli.Helpers;

namespace VPortal.GatewayClient.Host.Cli
{
    internal class GatewayClientHostCliProcessor : ISingletonDependency
    {
        private readonly IEnumerable<CliParameter> parameters;

        public GatewayClientHostCliProcessor(IEnumerable<CliParameter> parameters)
        {
            this.parameters = parameters;
        }

        public async Task ProcessArguments(string[] args)
        {
            await CliExceptionHandler.ExecuteWithHandling(async () => await ValidateAndProcess(args));
        }

        private async Task ValidateAndProcess(string[] args)
        {
            var parser = new CliArgumentsParser(parameters);
            var arguments = parser.ParseArguments(args);

            ValidateNoConflicts(arguments);

            var orderedByPriority = arguments.OrderByDescending(x => ParametersPriority.GetPriority(x.GetType()));
            foreach (var arg in orderedByPriority)
            {
                await arg.Execute();
            }
        }

        private void ValidateNoConflicts(IEnumerable<CliArgument> arguments)
        {
            foreach (var arg in arguments)
            {
                var t = arg.GetType();
                var conflict = arguments.FirstOrDefault(x => x.ForParameter.IncompatibleWithParameters.Contains(t));
                if (conflict != null)
                {
                    throw new ArgumentException($"You can not use both {arg.ParameterName} with {conflict.ParameterName}.");
                }
            }
        }
    }
}
