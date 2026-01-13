using Eleon.Storage.Lib.Models;
using Eleon.Storage.Module.Eleon.Storage.Module.Application.Contracts.StorageProviders;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Entities;
using EleonsoftModuleCollector.Storage.Module.Storage.Module.Application.Contracts.StorageProviders;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using SharedModule.modules.Blob.Module.Models;
using Storage.Module.StorageProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.DynamicOptions;
using VPortal.Storage.Module.Entities;

namespace VPortal.Storage.Module.StorageProviders
{
  [Authorize]
  public class StorageProvidersAppService : ModuleAppService, IStorageProvidersAppService
  {
    private readonly IVportalLogger<StorageProvidersAppService> logger;
    private readonly StorageProviderDomainService domainService;
    private readonly IDistributedEventBus eventBus;
    public StorageProvidersAppService(
        IVportalLogger<StorageProvidersAppService> logger,
        StorageProviderDomainService domainService,
        IDistributedEventBus eventBus)
    {
      this.logger = logger;
      this.domainService = domainService;
      this.eventBus = eventBus;
    }

    public async Task<StorageProviderDto> CreateStorageProvider(CreateStorageProviderDto input)
    {
      StorageProviderDto response = null;
      try
      {
        var entity = await domainService.CreateAsync(input.Name, input.TypeName);
        response = ObjectMapper.Map<StorageProviderEntity, StorageProviderDto>(entity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<List<PossibleStorageProviderSettingsDto>> GetPossibleSettings()
    {
      List<PossibleStorageProviderSettingsDto> response = null;
      try
      {
        var settings = await domainService.GetPossibleSettings();
        var mappedSettings = settings.Select(pair => new PossibleStorageProviderSettingsDto
        {
          PossibleSettings = ObjectMapper.Map<List<StorageProviderSettingTypeEntity>, List<StorageProviderSettingTypeDto>>(pair.Value),
          Type = pair.Key,
        });
        response = mappedSettings.ToList();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<StorageProviderDto> GetStorageProvider(string storageProviderId)
    {
      StorageProviderDto response = null;

      try
      {
        var gotEntity = await domainService.GetStorageProvider(storageProviderId);
        response = ObjectMapper.Map<StorageProviderEntity, StorageProviderDto>(gotEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<List<StorageProviderDto>> GetStorageProvidersList(StorageProviderListRequestDto input)
    {
      List<StorageProviderDto> response = null;

      try
      {
        var entityList = await domainService.GetStorageProvidersList(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.SearchQuery);
        response = ObjectMapper.Map<List<StorageProviderEntity>, List<StorageProviderDto>>(entityList);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<List<StorageProviderTypeDto>> GetStorageProviderTypesList()
    {
      {
        List<StorageProviderTypeDto> response = null;
        try
        {
          var types = await domainService.GetStorageProviderTypesList();
          response = ObjectMapper.Map<List<StorageProviderTypeEntity>, List<StorageProviderTypeDto>>(types);
        }
        catch (Exception e)
        {
          logger.Capture(e);
        }
        return response;
      }
    }

    public async Task<bool> RemoveStorageProvider(string storageProviderId)
    {
      bool response = false;
      try
      {
        response = await domainService.RemoveStorageProvider(storageProviderId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<StorageProviderSaveResponseDto> SaveStorageProvider(StorageProviderDto provider)
    {
      StorageProviderSaveResponseDto response = null;
      try
      {
        foreach (var setting in provider.Settings)
        {
          setting.Id = GuidGenerator.Create();
        }

        var entity = ObjectMapper.Map<StorageProviderDto, StorageProviderEntity>(provider);

        var updatedEntity = await domainService.SaveStorageProvider(entity);

        response = ObjectMapper.Map<StorageProviderEntity, StorageProviderSaveResponseDto>(updatedEntity);

        await eventBus.PublishAsync(new StorageProviderSettingsChangedMsg
        {
          StorageProviderId = provider.Id.ToString()
        });
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }
  }
}
