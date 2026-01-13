using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.Collaboration.Feature.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(CollaborationDomainSharedModule)
)]
public class CollaborationDomainModule : AbpModule
{
  public override void PreConfigureServices(ServiceConfigurationContext context)
  {
  }

  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<CollaborationDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<CollaborationDomainModule>(validate: true);
    });
  }
}

