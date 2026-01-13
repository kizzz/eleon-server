using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Domain.Shared.Auth
{
    public class GatewayClientRegistrationStages
    {
        public const string NotRegistered = "NotRegistered";
        public const string Registered = "Registered";
        public const string Confirmed = "Confirmed";
        public const string Pending = "Pending";
        public const string Completed = "Completed";
    }
}
