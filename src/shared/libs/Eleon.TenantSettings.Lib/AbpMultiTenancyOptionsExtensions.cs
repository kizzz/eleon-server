using Volo.Abp.MultiTenancy;

namespace Authorization.Module.TenantHostname
{
  public static class AbpMultiTenancyOptionsExtensions
  {
    public static void AddHostnameTenantResolver(this AbpTenantResolveOptions options)
    {
      options.TenantResolvers.InsertAfter(
          r => r is CurrentUserTenantResolveContributor,
          new HostnameTenantResolveContributor());
    }
  }
}
