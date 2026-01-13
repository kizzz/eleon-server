using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using VPortal.Storage.Module;

namespace VPortal.Storage.Remote.Application.Contracts;

[DependsOn(
    typeof(ProvidersDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule))]
public class StorageRemoteApplicationContractsModule : AbpModule
{ }
