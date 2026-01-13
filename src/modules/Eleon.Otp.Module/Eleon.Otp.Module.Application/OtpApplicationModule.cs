using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Otp.Module;

[DependsOn(
    typeof(OtpDomainModule),
    typeof(OtpApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class OtpApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<OtpApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<OtpApplicationModule>(validate: true);
    });
  }
}
