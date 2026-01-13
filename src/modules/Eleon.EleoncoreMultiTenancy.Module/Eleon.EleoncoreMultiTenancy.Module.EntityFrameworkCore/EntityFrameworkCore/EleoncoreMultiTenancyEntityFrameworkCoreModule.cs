using System.Collections.Generic;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ModuleCollector.EleoncoreMultiTenancyModule.EleoncoreMultiTenancy.Module.EntityFrameworkCore.Overrides;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

namespace EleoncoreMultiTenancy.Module.EntityFrameworkCore;

[DependsOn(
    typeof(EleoncoreMultiTenancyDomainModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class EleoncoreMultiTenancyEntityFrameworkCoreModule : AbpModule
{

  public override void ConfigureServices(ServiceConfigurationContext context)
  {

    context.Services.AddAbpDbContext<Volo.Abp.TenantManagement.EntityFrameworkCore.EleoncoreTenantManagementDbContext>(options =>
    {
    });

    context.Services.AddTransient(typeof(IDbContextProvider<>), typeof(CustomDbContextProvider<>));
  }
}
