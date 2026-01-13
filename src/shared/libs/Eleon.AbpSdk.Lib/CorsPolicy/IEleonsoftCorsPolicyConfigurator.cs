using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Eleoncore.Module.TenantHostname
{
  public interface IEleonsoftCorsPolicyConfigurator
  {
    void ConfigurePolicies(CorsPolicyBuilder builder);
  }
}
