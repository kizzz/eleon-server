using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal;
using VPortal.EntityFrameworkCore;

namespace Eleon.VPortal.Module.VPortalModule;

[DependsOn(
    typeof(VPortalApplicationModule),
    typeof(VPortalEntityFrameworkCoreModule),
    typeof(VPortalHttpApiModule),
    typeof(VPortalHttpApiClientModule)
    )]
public class VPortalModuleCollector : AbpModule
{
}
