using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.Domain.Shared.Auth
{
    public class ProxyClientRegistrationStages
    {
        public const string NotRegistered = "NotRegistered";
        public const string Registered = "Registered";
        public const string Confirmed = "Confirmed";
        public const string Completed = "Completed";
    }
}
