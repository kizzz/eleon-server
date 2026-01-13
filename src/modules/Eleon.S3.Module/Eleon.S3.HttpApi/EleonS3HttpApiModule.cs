using EleonS3.Application.Contracts;
using EleonS3.HttpApi.SigV4;
using Microsoft.AspNetCore.Builder;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace EleonS3.HttpApi;

[DependsOn(
    typeof(AbpAspNetCoreMvcModule),
    typeof(EleonS3ApplicationContractsModule)
)]
public class EleonS3HttpApiModule : AbpModule
{
}
