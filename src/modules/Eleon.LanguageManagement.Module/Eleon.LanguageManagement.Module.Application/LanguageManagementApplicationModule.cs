using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.LanguageManagement.Module;

[DependsOn(
    typeof(LanguageManagementDomainModule),
    typeof(LanguageManagementApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class LanguageManagementApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<LanguageManagementApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<LanguageManagementApplicationModule>(validate: true);
    });
  }
}

