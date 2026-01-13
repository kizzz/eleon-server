using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.GatewayClient.Domain.Shared.Constants;

namespace VPortal.GatewayClient.Domain.Shared.Helpers
{
    public class ExeHelper
    {
        public static string? GetHostExecutablePath()
        {
            string hostAssemblyName = GatewayClientHostConsts.HostAssemblyName;
            var possibleHostExeNames = new string[]
            {
                $"{hostAssemblyName}.exe",
                $"{hostAssemblyName}.EXE",
            };

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var possibleName in possibleHostExeNames)
            {
                string exePath = Path.Combine(basePath, possibleName);
                if (File.Exists(exePath))
                {
                    return exePath;
                }
            }

            return null;
        }
    }
}
