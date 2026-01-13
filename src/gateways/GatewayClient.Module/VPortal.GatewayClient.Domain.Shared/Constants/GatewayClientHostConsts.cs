using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Domain.Shared.Constants
{
    public class GatewayClientHostConsts
    {
        public static string HostAssemblyName => "VPortal.GatewayClient.Host";

        public static string HostServiceName => "VPortalGatewayClient";

        public static string HostMutexName => "Global\\VPortalGatewayClient";
    }
}
