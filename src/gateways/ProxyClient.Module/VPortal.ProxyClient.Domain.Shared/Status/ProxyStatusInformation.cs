using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.ProxyClient.Domain.Shared.Auth;

namespace VPortal.ProxyClient.Domain.Shared.Status
{
    public class ProxyStatusInformation
    {
        private static StringSplitOptions splitOpt = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

        private static List<(string regStage, string prettified)> prettyStages = new()
        {
            (ProxyClientRegistrationStages.NotRegistered, "Not Registered"),
            (ProxyClientRegistrationStages.Registered, "Registered"),
            (ProxyClientRegistrationStages.Completed, "Completed"),
            (ProxyClientRegistrationStages.Confirmed, "Confirmed"),
        };

        public ProxyStatusInformation(string registrationStage, int? port)
        {
            RegistrationStage = registrationStage;
            Port = port;
        }

        public static ProxyStatusInformation Parse(string stringifiedInfo)
        {
            var parts = stringifiedInfo.Split('\n', splitOpt);

            string regStage = null;
            try
            {
                string regStagePretty = parts[1].Split(':', splitOpt).Last();
                regStage = GetStage(regStagePretty);
            }
            catch
            {
            }

            if (regStage == null)
            {
                throw new ArgumentException("Could not parse the string as a Proxy Status Information", nameof(stringifiedInfo));
            }

            int? port = null;
            try
            {
                port = int.Parse(parts[2].Split(':', splitOpt).Last());
            }
            catch (Exception)
            {
            }

            return new ProxyStatusInformation(regStage, port);
        }

        public string RegistrationStage { get; }
        public int? Port { get; }

        public override string ToString()
            => $"\tRegistration: {GetPrettyStage(RegistrationStage)}\n" +
            $"\tPort: {Port?.ToString() ?? "Not Configured"}\n";

        private static string GetPrettyStage(string stage) => prettyStages.First(x => x.regStage == stage).prettified;
        private static string GetStage(string prettyStage) => prettyStages.First(x => x.prettified == prettyStage).regStage;
    }
}
