using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace VPortal.Otp.Module;

[DependsOn(
    typeof(OtpApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class OtpHttpApiClientModule : AbpModule
{
  public override void ConfigureServices(ServiceConfigurationContext context)
  {
    context.Services.AddHttpClientProxies(
        typeof(OtpApplicationContractsModule).Assembly,
        OtpRemoteServiceConsts.RemoteServiceName
    );

    Configure<AbpVirtualFileSystemOptions>(options =>
    {
      options.FileSets.AddEmbedded<OtpHttpApiClientModule>();
    });
  }
}
