using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Eleoncore.Module.TenantHostname
{
  public class EleonsoftCorsPolicyConfigurator(Action<CorsPolicyBuilder> action) : IEleonsoftCorsPolicyConfigurator
  {
    public void ConfigurePolicies(CorsPolicyBuilder builder)
    {
      action(builder);
    }
  }
}
