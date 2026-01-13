using Volo.Abp.Modularity;

namespace VPortal.ExternalLinkModule;

[DependsOn(
    typeof(VPortal.ExternalLink.Module.ModuleHttpApiModule),
    typeof(VPortal.ExternalLink.Module.ModuleApplicationModule),
    typeof(VPortal.ExternalLink.Module.EntityFrameworkCore.ExternalLinkEntityFrameworkCoreModule))]
public class ExternalLinkModuleCollector : AbpModule
{ }
