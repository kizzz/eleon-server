using Eleoncore.SDK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;

namespace VPortal.SdkAuthorization
{

  [Dependency(ReplaceServices = true)]
  public class CustomAbpAuthorizationPolicyProvider : EleoncoreSdkPolicyProvider, IAbpAuthorizationPolicyProvider, ITransientDependency
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
