using Eleon.Auth.App;
using Eleon.InternalProxy.App;
using Eleon.Storage.Module.Eleon.Storage.Module;
using Eleon.Templating.Module.Eleon.Templating.Module;
using EleoncoreMultiTenancy.Module;
using Volo.Abp.Modularity;
using VPortal;
using VPortal.Accounting.Module;
using VPortal.ApplicationConfiguration.Module;
using VPortal.Auditor.Module;
using VPortal.BackgroundJobs.Module;
using VPortal.Collaboration.Feature.Module;
using VPortal.DocMessageLogModule;
using VPortal.EventManagementModule;
using VPortal.ExternalLinkModule;
using VPortal.FileManagerModule;
using VPortal.GatewayManagement.Module;
using VPortal.Google.Module;
using VPortal.HealthCheckModule;
using VPortal.Identity.Module;
using VPortal.Infrastructure.Module;
using VPortal.JobScheduler.Module;
using VPortal.LanguageManagement.Module;
using VPortal.Lifecycle.Feature.Module;
using VPortal.NotificatorModule;
using VPortal.Otp.Module;
using VPortal.SitesManagement.Module;
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

    typeof(BackgroundJobsModuleCollector),
    typeof(JobSchedulerModuleCollector),
    typeof(AccountingModuleCollector),
    typeof(AuditorModuleCollector),
    typeof(CollaborationModuleCollector),
    typeof(EleoncoreMultiTenancyModule),
    typeof(FileManagerModuleCollector),
    typeof(EventManagementModuleCollector),
    typeof(LifecycleFeatureModuleCollector),
    typeof(EleonTemplatingModuleCollector),
    typeof(GoogleModuleCollector),
    typeof(HealthCheckModuleCollector),

    // auth modules
    typeof(AuthMvcModule),
    typeof(OtpModuleCollector),
    typeof(ExternalLinkModuleCollector),
    typeof(ApplicationConfigurationModuleCollector),
    typeof(IdentityQueryingModuleCollector),
    typeof(SystemServicesModuleCollector),

    // eleoncore
    typeof(SitesManagementModuleCollector),
    typeof(GatewayManagementModuleCollector),

    // proxy module
    typeof(InternalProxyModule)
    )]
public class MaximalHostModule : AbpModule
{
}
