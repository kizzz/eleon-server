using Duende.IdentityModel.Client;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Volo.Abp.IdentityModel;

namespace VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants
{
  public interface IExtensionGrantTokenRequestProvider
  {
    [NotNull]
    string GrantType { get; }

    Task<TokenRequest> GetTokenRequest(IdentityClientConfiguration configuration);
  }
}
