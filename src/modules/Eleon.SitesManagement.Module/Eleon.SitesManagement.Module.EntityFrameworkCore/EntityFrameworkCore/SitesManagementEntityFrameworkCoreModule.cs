using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow.EntityFrameworkCore;
using VPortal.SitesManagement.Module;

namespace VPortal.SitesManagement.Module.EntityFrameworkCore;

[DependsOn(
    typeof(SitesManagementDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class SitesManagementEntityFrameworkCoreModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
    SitesManagementEfCoreEntityExtensionMappings.Configure();
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAbpDbContext<SitesManagementDbContext>(options =>
    {
    });
  }
}


