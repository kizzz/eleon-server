using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.Domain.Shared.Constants
{
    public class ProxyClientHostConsts
    {
        public static string HostAssemblyName => "VPortal.ProxyClient.Host";

        public static string HostServiceName => "VPortalProxyClient";

        public static string HostMutexName => "Global\\VPortalProxyClient";
    }
}
