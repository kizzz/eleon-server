using Common.EventBus.Module;
using Common.Module.Events;
using Commons.Module.Messages.Storage;
using Logging.Module;
using Messaging.Module.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Storage.Module.DomainServices;

namespace AdministrationModule.Storage.Module.Storage.Module.Application.EventServices;
public class StorageEventService : IDistributedEventHandler<SaveToStorageMsg>, IDistributedEventHandler<GetFromStorageMsg>, ITransientDependency
{
  private readonly IVportalLogger<StorageEventService> _logger;
  private readonly StorageDomainService _domainService;
  private readonly IResponseContext _responseContext;

  public StorageEventService(
      IVportalLogger<StorageEventService> logger,
      StorageDomainService domainService,
      IResponseContext responseContext)
  {
    _logger = logger;
    _domainService = domainService;
    _responseContext = responseContext;
  }

  public async Task HandleEventAsync(SaveToStorageMsg eventData)
  {

    var response = new SaveToStorageResponseMsg { Success = false };

    try
    {
      var data = Convert.FromBase64String(eventData.Base64Data);
      response.Success = await _domainService.Save(eventData.SettingsGroup, eventData.BlobName, data, eventData.OverwriteIfExists);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }

  public async Task HandleEventAsync(GetFromStorageMsg eventData)
  {

    var response = new GetFromStorageResponseMsg { Success = false };

    try
    {
      var data = await _domainService.GetBytes(eventData.SettingsGroup, eventData.BlobName);
      response.Success = true;
      response.Base64Data = Convert.ToBase64String(data);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
