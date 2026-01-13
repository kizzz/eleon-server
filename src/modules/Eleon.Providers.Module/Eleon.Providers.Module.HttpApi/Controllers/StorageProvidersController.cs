using Eleon.Storage.Lib.Models;
using Eleon.Storage.Module.Eleon.Storage.Module.Application.Contracts.StorageProviders;
using EleonsoftModuleCollector.Storage.Module.Storage.Module.Application.Contracts.StorageProviders;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using SharedModule.modules.Blob.Module.Models;
using Storage.Module.StorageProviders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Storage.Module.StorageProviders;

namespace VPortal.Storage.Module.Controllers;

[Area(ProvidersRemoteServiceConsts.ModuleName)]
[RemoteService(Name = ProvidersRemoteServiceConsts.RemoteServiceName)]
[Route("api/Storage/StorageProviders")]
public class StorageProvidersController : ModuleController, IStorageProvidersAppService
{
  private readonly IStorageProvidersAppService appService;
  private readonly IVportalLogger<StorageProvidersController> _logger;

  public StorageProvidersController(
      IStorageProvidersAppService appService,
      IVportalLogger<StorageProvidersController> logger)
  {
    this.appService = appService;
    _logger = logger;
  }

  [HttpPost("CreateStorageProvider")]
  public Task<StorageProviderDto> CreateStorageProvider(CreateStorageProviderDto input)
  {
    var response = appService.CreateStorageProvider(input);
    return response;
  }

  [HttpGet("GetPossibleSettings")]
  public async Task<List<PossibleStorageProviderSettingsDto>> GetPossibleSettings()
  {

    var response = await appService.GetPossibleSettings();


    return response;
  }

  [HttpGet("GetStorageProvider")]
  public async Task<StorageProviderDto> GetStorageProvider(string storageProviderId)
  {

    var response = await appService.GetStorageProvider(storageProviderId);


    return response;
  }

  [HttpGet("GetStorageProvidersList")]
  public async Task<List<StorageProviderDto>> GetStorageProvidersList(StorageProviderListRequestDto input)
  {

    var response = await appService.GetStorageProvidersList(input);


    return response;
  }

  [HttpGet("GetStorageProviderTypesList")]
  public async Task<List<StorageProviderTypeDto>> GetStorageProviderTypesList()
  {

    var response = await appService.GetStorageProviderTypesList();

    return response;
  }


  [HttpPost("RemoveStorageProvider")]
  public async Task<bool> RemoveStorageProvider(string storageProviderId)
  {

    var response = await appService.RemoveStorageProvider(storageProviderId);


    return response;
  }

  [HttpPost("SaveStorageProvider")]
  public async Task<StorageProviderSaveResponseDto> SaveStorageProvider(StorageProviderDto provider)
  {

    var response = await appService.SaveStorageProvider(provider);


    return response;
  }
}
