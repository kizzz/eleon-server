using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Eleoncore.Module.TenantHostname
{
  public class EleoncoreCorsPolicyConfigurator(Action<CorsPolicyBuilder> action) : IEleoncoreCorsPolicyConfigurator
  {
    public void ConfigurePolicies(CorsPolicyBuilder builder)
    {
      action(builder);
    }
  }
}
