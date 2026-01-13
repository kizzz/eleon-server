using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants.MachineKey;
using Common.Module.Helpers;
using VPortal.ProxyClient.Domain.Shared.Auth;
using VPortal.ProxyClient.Domain.Storage;

namespace VPortal.ProxyClient.Domain.Auth
{
    [ExposeServices(typeof(IMachineSecretsProvider), IncludeSelf = true)]
    public class MachineSecretsProvider : IMachineSecretsProvider, ITransientDependency
    {
        public MachineSecretsProvider()
        {
        }

        public async Task<string> GetClientCompoundKey()
        {
            return LicenseHelper.License.ClientCompoundKey;
        }

        public async Task<string> GetMachineKey()
        {
            return MachineKeyHelper.GetMachineKeyHash();
        }
    }
}
