using EleonCore.Modules.S3;
using EleonCore.Modules.S3.EntityFrameworkCore;
using EleonS3.HttpApi;
using Volo.Abp.Modularity;

namespace EleonS3Module;

[DependsOn(
    typeof(EleonS3ApplicationModule),
    typeof(EleonS3HttpApiModule))]
public class EleonS3ModuleCollector : AbpModule
{
}
