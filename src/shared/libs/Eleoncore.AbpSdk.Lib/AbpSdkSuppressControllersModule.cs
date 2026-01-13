using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using VPortal.Policies;

namespace abp_sdk
{
  public class AbpSdkSuppressControllersModule : AbpModule
  {
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
      base.ConfigureServices(context);

      Configure<MvcOptions>(options =>
      {
        options.Conventions.Add(new RemoveAbpControllersConventions());
      });
    }
  }
}
