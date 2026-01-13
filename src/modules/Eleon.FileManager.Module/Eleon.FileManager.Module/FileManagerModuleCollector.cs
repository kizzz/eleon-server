using Volo.Abp.Modularity;

namespace VPortal.FileManagerModule;

[DependsOn(
    typeof(VPortal.FileManager.Module.ModuleHttpApiModule),
    typeof(VPortal.FileManager.Module.ModuleApplicationModule),
    typeof(VPortal.FileManager.Module.EntityFrameworkCore.FileManagerEntityFrameworkCoreModule))]
public class FileManagerModuleCollector : AbpModule
{ }
