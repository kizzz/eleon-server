using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.ProxyClient.Domain.Shared.Constants;

namespace VPortal.ProxyClient.Domain.Shared.Helpers
{
    public class ExeHelper
    {
        public static string? GetHostExecutablePath()
        {
            string hostAssemblyName = ProxyClientHostConsts.HostAssemblyName;
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
