using Common.EventBus.Module;
using Eleon.Storage.Module.Eleon.Storage.Module.Domain.Shared.Messages;
using EleonsoftSdk.modules.Messaging.Module.Messages.SystemHealth;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.StorageProvider;
using Logging.Module;
using SharedModule.modules.Blob.Module.Constants;
using SharedModule.modules.Blob.Module.Models;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.Storage.Module.DomainServices;
using VPortal.Storage.Module.Entities;

namespace EleonsoftModuleCollector.Storage.Module.Storage.Module.Application.StorageProviders;
public class StorageProviderEventHandler : IDistributedEventHandler<GetStorageProviderMsg>, IDistributedEventHandler<GetMinimalStorageProvidersListMsg>, ITransientDependency
{
  private readonly IResponseContext _responseContext;
  private readonly StorageProviderDomainService _storageProviderDomainService;
  private readonly IVportalLogger<StorageProviderEventHandler> _logger;
  private readonly IDistributedEventBus _tenantSettingsDomainService;
  private readonly IObjectMapper _objectMapper;

  public StorageProviderEventHandler(
      IResponseContext responseContext,
      StorageProviderDomainService storageProviderDomainService,
      IDistributedEventBus tenantSettingsDomainService,
      IVportalLogger<StorageProviderEventHandler> logger,
      IObjectMapper objectMapper)
  {
    _responseContext = responseContext;
    _storageProviderDomainService = storageProviderDomainService;
    _logger = logger;
    _tenantSettingsDomainService = tenantSettingsDomainService;
    _objectMapper = objectMapper;
  }

  public async Task HandleEventAsync(GetStorageProviderMsg eventData)
  {
    StorageProviderDto response = null;
    try
    {
      var storageProviderId = eventData.StorageProviderId;
      if (storageProviderId == BlobMessagingConsts.TelemetryStorageProviderKey)
      {
        var settings = await _tenantSettingsDomainService.RequestAsync<SystemHealthSettingsResponseMsg>(new SystemHealthSettingsRequestMsg { });
        storageProviderId = settings.StorageProviderId.ToString();
      }

      var storageProvider = await _storageProviderDomainService.GetStorageProvider(storageProviderId);
      response = _objectMapper.Map<StorageProviderEntity, StorageProviderDto>(storageProvider);

    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(new GetStorageProviderResponseMsg
      {
        StorageProvider = response
      });
    }
  }

  public async Task HandleEventAsync(GetMinimalStorageProvidersListMsg eventData)
  {
    var response = new GetMinimalStorageProvidersListResponseMsg();

    try
    {
      var providers = await _storageProviderDomainService.GetStorageProvidersListByIds(eventData.ProviderIds);
      response.StorageProviders = _objectMapper.Map<List<StorageProviderEntity>, List<MinimalStorageProviderDto>>(providers);
      response.IsSuccess = true;
    }
    catch (Exception ex)
    {
      response.IsSuccess = false;
      _logger.Capture(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}

