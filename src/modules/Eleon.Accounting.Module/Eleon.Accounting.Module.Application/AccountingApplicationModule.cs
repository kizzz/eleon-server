using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace VPortal.Accounting.Module;

[DependsOn(
    typeof(AccountingDomainModule),
    typeof(AccountingApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class AccountingApplicationModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<AccountingApplicationModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<AccountingApplicationModule>(validate: true);
    });
  }
}
