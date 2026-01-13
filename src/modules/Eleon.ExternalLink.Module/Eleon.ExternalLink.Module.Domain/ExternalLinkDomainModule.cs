using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.ExternalLink.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(ModuleDomainSharedModule))]
public class ExternalLinkDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<ExternalLinkDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<ExternalLinkDomainModule>(validate: true);
    });
  }
}

