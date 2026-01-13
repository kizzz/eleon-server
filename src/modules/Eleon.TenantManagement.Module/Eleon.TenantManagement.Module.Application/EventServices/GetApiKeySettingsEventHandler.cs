using Common.EventBus.Module;
using EleonsoftAbp.Messages.ApiKey;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.Identity.Module.DomainServices;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.EventServices;
public class GetApiKeySettingsEventHandler : IDistributedEventHandler<ApiKeyRequestMsg>, ITransientDependency
{
  private readonly ApiKeyDomainService _apiKeyDomainService;
  private readonly IVportalLogger<GetApiKeySettingsEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly IUnitOfWorkManager _unitOfWorkManager;

  public GetApiKeySettingsEventHandler(
      ApiKeyDomainService apiKeyDomainService,
      IVportalLogger<GetApiKeySettingsEventHandler> logger,
      IResponseContext responseContext,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _apiKeyDomainService = apiKeyDomainService;
    _logger = logger;
    _responseContext = responseContext;
    _unitOfWorkManager = unitOfWorkManager;
  }

  public async Task HandleEventAsync(ApiKeyRequestMsg eventData)
  {

    var response = new ApiKeyResponseMsg
    {
      Name = null,
      KeyId = null,
      AllowAuthorize = false,
      ExpiredAt = null,
      Data = null,
      RefId = null,
      Found = false,
    };
    try
    {
      using var uow = _unitOfWorkManager.Begin();
      var key = await _apiKeyDomainService.FindByIdAsync(Guid.Parse(eventData.KeyId));

      if (key == null)
      {
        _logger.Log.LogWarning($"API key with ID {eventData.KeyId} not found.");
        return;
      }

      response = new ApiKeyResponseMsg
      {
        Name = key.Name,
        KeyId = key.Id.ToString(),
        AllowAuthorize = key.AllowAuthorize,
        ExpiredAt = key.ExpiresAt,
        Data = key.Data,
        RefId = key.RefId,
        Found = true,
      };
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
