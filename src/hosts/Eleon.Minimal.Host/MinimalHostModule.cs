using Eleon.Auth.App;
using Eleon.InternalProxy.App;
using Eleon.Storage.Module.Eleon.Storage.Module;
using Eleon.Templating.Module.Eleon.Templating.Module;
using EleoncoreMultiTenancy.Module;
using Volo.Abp.Modularity;
using VPortal;
using VPortal.ApplicationConfiguration.Module;
using VPortal.BackgroundJobs.Module;
using VPortal.DocMessageLogModule;
using VPortal.ExternalLinkModule;
using VPortal.FileManagerModule;
using VPortal.HealthCheckModule;
using VPortal.Infrastructure.Module;
using VPortal.JobScheduler.Module;
using VPortal.LanguageManagement.Module;
using VPortal.NotificatorModule;
using VPortal.Otp.Module;
using VPortal.Storage.Module;
using VPortal.SystemServicesModule;
using VPortal.TenantManagement.Module;

namespace Eleonsoft.Host;

[DependsOn(
    typeof(EleonHostModule),

    // admin modules
    typeof(EleonAbpEfModuleCollector),
    typeof(InfrastructureModuleCollector),
    typeof(TenantManagementModuleCollector),
    typeof(SystemLogModuleCollector),
    typeof(NotificatorModuleCollector),
    typeof(LanguageManagementModuleCollector),
    typeof(ProvidersModuleCollector),
    typeof(EleonStorageModuleCollector),
    typeof(EleonTemplatingModuleCollector),

    // auth modules
    typeof(AuthMvcModule),
    typeof(OtpModuleCollector),
    typeof(ExternalLinkModuleCollector),
    typeof(ApplicationConfigurationModuleCollector),
    typeof(IdentityQueryingModuleCollector),
    typeof(SystemServicesModuleCollector),

    // immu required modules
    typeof(FileManagerModuleCollector),
    typeof(BackgroundJobsModuleCollector),
    typeof(JobSchedulerModuleCollector),

    // proxy module
    typeof(InternalProxyModule)
    )]
public class MinimalHostModule : AbpModule
{
}
