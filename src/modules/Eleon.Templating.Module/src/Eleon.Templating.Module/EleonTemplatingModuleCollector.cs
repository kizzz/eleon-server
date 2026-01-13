using Eleon.Templating.Module.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;

namespace Eleon.Templating.Module.Eleon.Templating.Module
{
  [DependsOn(typeof(TemplatingHttpApiModule),
    typeof(TemplatingApplicationModule),
    typeof(TemplatingEntityFrameworkCoreModule))]
  public class EleonTemplatingModuleCollector : AbpModule
  {
  }
}
