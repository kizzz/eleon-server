using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using SharedModule.modules.Blob.Module.Models;
using System;
using System.Threading.Tasks;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.Entities;

namespace VPortal.Storage.Module.StorageProviders
{
  [Authorize]
  public class StorageProvidersTestAppService : ModuleAppService, IStorageProvidersTestAppService
  {
    private readonly IVportalLogger<StorageProvidersTestAppService> logger;
    private readonly StorageProviderDomainService domainService;

    public StorageProvidersTestAppService(
        IVportalLogger<StorageProvidersTestAppService> logger,
        StorageProviderDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<bool> TestStorageProvider(StorageProviderDto provider)
    {
      bool response = false;
      try
      {
        //    if (provider.Id == Guid.Empty)
        //    {
        //        provider.Id = GuidGenerator.Create();
        //    }

        //    foreach (var setting in provider.Settings)
        //    {
        //        if (setting.Id == Guid.Empty)
        //        {
        //            setting.Id = GuidGenerator.Create();
        //        }
        //    }

        //    var entity = ObjectMapper.Map<StorageProviderDto, StorageProviderEntity>(provider);
        //    response = await domainService.TestStorageProvider(entity);
        //

      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }
  }
}
