using Volo.Abp.Modularity;

namespace VPortal.DocMessageLogModule;

[DependsOn(
    typeof(VPortal.DocMessageLog.Module.ModuleHttpApiModule),
    typeof(VPortal.DocMessageLog.Module.ModuleApplicationModule),
    typeof(VPortal.DocMessageLog.Module.EntityFrameworkCore.SystemLogEntityFrameworkCoreModule))]
public class SystemLogModuleCollector : AbpModule
{ }
