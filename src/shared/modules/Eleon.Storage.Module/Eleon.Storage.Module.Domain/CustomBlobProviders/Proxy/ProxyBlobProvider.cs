using Logging.Module;
//using ProxyManagement.Module.ProxyCallInterception;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.DependencyInjection;
//using VPortal.Storage.Remote.Application.Contracts.Storage;

namespace Storage.Module.BlobProviders.Proxy
{
  //public class ProxyBlobProvider : BlobProviderBase, ITransientDependency
  //{
  //    private readonly IVportalLogger<ProxyBlobProvider> logger;
  //    private readonly IProxyAppServiceProvider proxyAppServiceProvider;

  //    public ProxyBlobProvider(
  //        IVportalLogger<ProxyBlobProvider> logger,
  //        IProxyAppServiceProvider proxyAppServiceProvider)
  //    {
  //        this.logger = logger;
  //        this.proxyAppServiceProvider = proxyAppServiceProvider;
  //    }

  //    public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
  //    {

  //        var cfg = GetConfiguration(args);
  //        var appService = await GetAppService(cfg);
  //        bool result = await appService.Delete(cfg.SettingsGroup, args.BlobName);

  //        return result;
  //    }

  //    public override async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
  //    {

  //        var cfg = GetConfiguration(args);
  //        var appService = await GetAppService(cfg);
  //        bool result = await appService.Exists(cfg.SettingsGroup, args.BlobName);

  //        return result;
  //    }

  //    public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
  //    {

  //        var cfg = GetConfiguration(args);
  //        var appService = await GetAppService(cfg);
  //        var base64 = await appService.GetBase64(cfg.SettingsGroup, args.BlobName);
  //        var data = Convert.FromBase64String(base64);
  //        Stream result = new MemoryStream(data);

  //        return result;
  //    }

  //    public override async Task SaveAsync(BlobProviderSaveArgs args)
  //    {

  //        var cfg = GetConfiguration(args);
  //        var data = await args.BlobStream.GetAllBytesAsync();
  //        string base64 = Convert.ToBase64String(data);
  //        var appService = await GetAppService(cfg);
  //        bool success = await appService.Save(new SaveBase64Request()
  //        {
  //            BlobName = args.BlobName,
  //            SettingsGroup = cfg.SettingsGroup,
  //            DataBase64 = base64,
  //        });

  //        if (!success)
  //        {
  //            throw new Exception("Could not save blob to the proxy.");
  //        }

  //    }

  //    private IStorageRemoteAppService appServiceCache;
  //    async Task<IStorageRemoteAppService> GetAppService(ProxyBlobContainerConfiguration cfg)
  //    {
  //        return appServiceCache ??= await proxyAppServiceProvider.ResolveScopedProxyAppService<IStorageRemoteAppService>(cfg.ProxyId);
  //    }

  //    ProxyBlobContainerConfiguration GetConfiguration(BlobProviderArgs args)
  //        => args.Configuration.GetProxyBlobProviderConfiguration();
  //}
}
