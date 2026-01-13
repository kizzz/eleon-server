using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.ProxyClient.Host.Cli.Configuration
{
    public enum HostStartupType
    {
        None = 0,
        Console = 1,
        Service = 2,
    }

    public class HostStartupConfiguration : ISingletonDependency
    {
        public HostStartupType HostStartupType { get; set; } = HostStartupType.None;
    }
}
