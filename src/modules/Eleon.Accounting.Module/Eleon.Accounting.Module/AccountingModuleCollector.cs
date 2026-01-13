using Volo.Abp.Modularity;
using VPortal.Accounting.Module.EntityFrameworkCore;

namespace VPortal.Accounting.Module
{
  [DependsOn(
     typeof(AccountingHttpApiModule),
     typeof(AccountingApplicationModule),
     typeof(AccountingEntityFrameworkCoreModule)
 )]
  public class AccountingModuleCollector : AbpModule
  {

  }
}
