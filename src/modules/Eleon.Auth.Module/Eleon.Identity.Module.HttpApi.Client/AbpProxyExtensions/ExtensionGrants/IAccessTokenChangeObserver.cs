using System.Threading.Tasks;

namespace VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants
{
  public interface IAccessTokenChangeObserver
  {
    Task OnAccessTokenChangeAsync(string accessToken);
  }
}
