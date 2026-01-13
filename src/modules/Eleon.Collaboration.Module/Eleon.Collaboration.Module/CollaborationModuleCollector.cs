using Volo.Abp.Modularity;
using VPortal.Collaboration.Feature.Module.EntityFrameworkCore;

namespace VPortal.Collaboration.Feature.Module
{
  [DependsOn(
      typeof(CollaborationApplicationModule),
      typeof(CollaborationHttpApiModule),
      typeof(CollaborationEntityFrameworkCoreModule))]
  public class CollaborationModuleCollector : AbpModule
  {
  }
}
