using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.Host.Cli.Cli
{
    public class CliArgumentsParser
    {
        public static string[] ShortNameSeparators = new[] { "/", "-" };
        public static string[] LongNameSeparators = new[] { "--" };
        private readonly IEnumerable<CliParameter> possibleParams;

        public CliArgumentsParser(IEnumerable<CliParameter> possibleParams)
        {
            this.possibleParams = possibleParams;
        }

        public List<CliArgument> ParseArguments(string[] args)
        {
            var parsed = new List<CliArgument>();
            if (args.Length == 0)
            {
                return parsed;
            }

            var separators = ShortNameSeparators.Concat(LongNameSeparators).Select(x => ' ' + x).ToArray();
            string totalStr = ' ' + string.Join(' ', args);
            var paramsWithArgs = totalStr.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var paramWithArgs in paramsWithArgs)
            {
                var paramParts = paramWithArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var firstFit = possibleParams.FirstOrDefault(x => x.Fits(paramParts));
                if (firstFit == null)
                {
                    throw new ArgumentException($"Unknown parameter: '{totalStr}'");
                }

                parsed.Add(firstFit.Parse(paramParts));
            }

            return parsed;
        }
    }
}
