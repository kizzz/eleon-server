using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Host.Cli.Cli
{
    public abstract class CliParameter
    {
        public abstract string ShortName { get; }
        public abstract string LongName { get; }
        public abstract string Description { get; }
        public abstract string[] ValuesDescriptions { get; }
        public int ValuesAmount => ValuesDescriptions.Length;
        public virtual IEnumerable<Type> IncompatibleWithParameters { get; } = Array.Empty<Type>();

        public CliParameter()
        {
        }

        public abstract Task Execute(CliArgument arg);

        public bool Fits(string[] parts)
        {
            var first = parts.First();
            return first == ShortName || first == LongName;
        }

        public CliArgument Parse(string[] parts)
        {
            var paramName = parts.First();
            var args = parts.Skip(1).ToArray();
            if (args.Length < ValuesAmount)
            {
                throw new ArgumentException($"Not enough values for {paramName} parameter (expected: {ValuesAmount}, got: {args.Length}).");
            }

            if (args.Length > ValuesAmount)
            {
                throw new ArgumentException($"Too many values for {paramName} parameter (expected: {ValuesAmount}, got: {args.Length}).");
            }

            return new CliArgument(this, paramName, args);
        }
    }
}
