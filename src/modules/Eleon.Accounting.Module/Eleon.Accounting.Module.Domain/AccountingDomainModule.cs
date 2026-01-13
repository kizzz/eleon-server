using Common.Module.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.Accounting.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(AccountingDomainSharedModule)
)]
public class AccountingDomainModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {

  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<AccountingDomainModule>();
    context.Services.AddTransient<XmlSerializerHelper>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<AccountingDomainModule>(validate: true);
    });
  }
}

