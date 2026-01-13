using Eleon.Storage.Module.Eleon.Storage.Module;
using Eleon.Templating.Module.Eleon.Templating.Module;
using EleoncoreMultiTenancy.Module;
using Volo.Abp.Modularity;
using VPortal;
using VPortal.Accounting.Module;
using VPortal.Auditor.Module;
using VPortal.BackgroundJobs.Module;
using VPortal.Collaboration.Feature.Module;
using VPortal.DocMessageLogModule;
using VPortal.EventManagementModule;
using VPortal.FileManagerModule;
using VPortal.Google.Module;
using VPortal.HealthCheckModule;
using VPortal.Infrastructure.Module;
using VPortal.JobScheduler.Module;
using VPortal.LanguageManagement.Module;
using VPortal.Lifecycle.Feature.Module;
using VPortal.NotificatorModule;
using VPortal.Storage.Module;
using VPortal.TenantManagement.Module;

namespace Eleonsoft.Host;

[DependsOn(
    typeof(EleonHostModule),

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
    typeof(HealthCheckModuleCollector)
    )]
public class EleonsoftHostModule : AbpModule
{
}
