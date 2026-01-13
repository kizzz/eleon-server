using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;

namespace Eleoncore.SDK
{
  public class EleonsoftSdkPolicyProvider : DefaultAuthorizationPolicyProvider
  {
    private readonly AuthorizationOptions _options;

    public EleonsoftSdkPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
      _options = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
      var policy = await base.GetPolicyAsync(policyName);
      if (policy != null)
      {
        return policy;
      }

      var policyBuilder = new AuthorizationPolicyBuilder(Array.Empty<string>());
      policyBuilder.Requirements.Add(new EleonsoftSdkPermissionRequirement(policyName));
      return policyBuilder.Build();
    }
  }
}
