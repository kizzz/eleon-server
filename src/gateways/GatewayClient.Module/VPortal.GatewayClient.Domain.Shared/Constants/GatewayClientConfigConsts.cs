using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.Config
{
    public static class GatewayClientConfigConsts
    {
        public static string AppsettingsPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        public static string UiLogsDirectoryPath => Path.Combine(AppContext.BaseDirectory, "GatewayClientConfigLogs");
        public static string HostLogsPath => Path.Combine(AppContext.BaseDirectory, "HostLogs");
    }
}
