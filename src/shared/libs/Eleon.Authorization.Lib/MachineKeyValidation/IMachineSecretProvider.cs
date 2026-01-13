using System.Threading.Tasks;

namespace VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants.MachineKey
{
  public interface IMachineSecretsProvider
  {
    Task<string> GetClientCompoundKey();
    Task<string> GetMachineKey();
  }
}
