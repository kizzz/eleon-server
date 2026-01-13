using Volo.Abp.Modularity;
//using VPortal.Storage.Remote.HttpApi;
//using VPortal.Storage.Remote.HttpApi.Client;

namespace VPortal.Storage.Module;

[DependsOn(
    //typeof(StorageRemoteHttpApiClientModule),
    //typeof(StorageRemoteHttpApiModule),
    typeof(VPortal.Storage.Module.ProviderHttpApiModule),
    typeof(VPortal.Storage.Module.ProviderApplicationModule),
    typeof(VPortal.Storage.Module.EntityFrameworkCore.ProviderEntityFrameworkCoreModule))]
public class ProvidersModuleCollector : AbpModule
{
}
