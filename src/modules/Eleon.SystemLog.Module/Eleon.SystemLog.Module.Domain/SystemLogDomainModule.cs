using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace VPortal.DocMessageLog.Module;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(SystemLogDomainSharedModule))]
public class SystemLogDomainModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddAutoMapperObjectMapper<SystemLogDomainModule>();
    Configure<AbpAutoMapperOptions>(options =>
    {
      options.AddMaps<SystemLogDomainModule>(validate: true);
    });
  }
}

