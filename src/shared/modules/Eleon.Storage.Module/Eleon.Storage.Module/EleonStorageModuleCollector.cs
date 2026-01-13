using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using VPortal.Storage.Module;

namespace Eleon.Storage.Module.Eleon.Storage.Module;

[DependsOn(typeof(StorageApplicationModule), typeof(StorageHttpApiModule))]
public class EleonStorageModuleCollector : AbpModule
{
}
