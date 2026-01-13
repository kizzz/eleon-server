using Eleoncore.SDK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;

namespace VPortal.SdkAuthorization
{
  public class CustomAbpAuthorizationPolicyProvider : EleonsoftSdkPolicyProvider, IAbpAuthorizationPolicyProvider
  {
    private readonly AbpAuthorizationPolicyProvider abpProvider;

    public CustomAbpAuthorizationPolicyProvider(AbpAuthorizationPolicyProvider abpProvider, IOptions<AuthorizationOptions> options) : base(options)
    {
      this.abpProvider = abpProvider;
    }

    public Task<List<string>> GetPoliciesNamesAsync()
    {
      return abpProvider.GetPoliciesNamesAsync();
    }
  }
}
