using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Eleoncore.Module.TenantHostname
{
  public interface IEleoncoreCorsPolicyConfigurator
  {
    void ConfigurePolicies(CorsPolicyBuilder builder);
  }
}
