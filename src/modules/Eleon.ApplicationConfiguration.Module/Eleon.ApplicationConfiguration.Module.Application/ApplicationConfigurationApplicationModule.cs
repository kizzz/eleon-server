using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using VPortal.Otp;

namespace VPortal.ApplicationConfiguration.Module;

[DependsOn(
    typeof(ApplicationConfigurationDomainModule),
    typeof(ApplicationConfigurationApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class ApplicationConfigurationApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ApplicationConfigurationApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ApplicationConfigurationApplicationModule>(validate: true);
    });
  }
}
