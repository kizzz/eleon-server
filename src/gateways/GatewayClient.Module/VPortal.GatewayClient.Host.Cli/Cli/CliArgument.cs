using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Host.Cli.Cli
{
    public class CliArgument
    {
        public CliParameter ForParameter { get; }
        public IEnumerable<string> Values { get; }
        public string ParameterName { get; set; }

        public CliArgument(CliParameter forParameter, string parameterName, IEnumerable<string> values)
        {
            ForParameter = forParameter;
            Values = values;
            ParameterName = parameterName;
        }

        public async Task Execute()
            => await ForParameter.Execute(this);
    }
}
