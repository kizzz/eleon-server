using Eleon.Storage.Lib.Models;
using Eleon.Storage.Module.Eleon.Storage.Module.Application.Contracts.StorageProviders;
using EleonsoftModuleCollector.Storage.Module.Storage.Module.Application.Contracts.StorageProviders;
using SharedModule.modules.Blob.Module.Models;
using Storage.Module.StorageProviders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace VPortal.Storage.Module.StorageProviders
{
  public interface IStorageProvidersAppService : IApplicationService
  {
    Task<StorageProviderSaveResponseDto> SaveStorageProvider(StorageProviderDto provider);
    Task<StorageProviderDto> GetStorageProvider(string storageProviderId);
    Task<bool> RemoveStorageProvider(string storageProviderId);
    Task<List<StorageProviderDto>> GetStorageProvidersList(StorageProviderListRequestDto input);
    Task<List<PossibleStorageProviderSettingsDto>> GetPossibleSettings();
    Task<List<StorageProviderTypeDto>> GetStorageProviderTypesList();
    Task<StorageProviderDto> CreateStorageProvider(CreateStorageProviderDto input);
  }
}
