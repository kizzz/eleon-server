using System.Threading.Tasks;

namespace VPortal.Identity.Module.AbpProxyExtensions
{
  public interface IRemoteServiceTokenManager
  {
    Task ForgetAllTokens();
  }
}
