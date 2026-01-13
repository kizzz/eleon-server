using Common.EventBus.Module;
using EleonsoftSdk.Messages.Permissions;
using Logging.Module;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.EventServices;
public class CheckPermissionEventHandler : IDistributedEventHandler<CheckPermissionRequestMsg>, ITransientDependency
{
  private readonly IResponseContext _responseContext;
  private readonly IVportalLogger<CheckPermissionEventHandler> _logger;
  private readonly IPermissionManager _permissionManager;
  private readonly IUnitOfWorkManager _unitOfWorkManager;

  public CheckPermissionEventHandler(
      IResponseContext responseContext,
      IVportalLogger<CheckPermissionEventHandler> logger,
      IPermissionManager permissionManager,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _responseContext = responseContext;
    _logger = logger;
    _permissionManager = permissionManager;
    _unitOfWorkManager = unitOfWorkManager;
  }

  public async Task HandleEventAsync(CheckPermissionRequestMsg eventData)
  {

    var response = new CheckPermissionResponseMsg
    {
      IsSuccessful = false,
      PermissionGrants = eventData.Permissions.ToDictionary(x => x, _ => false),
      ErrorMessage = string.Empty
    };

    try
    {
      using var uow = _unitOfWorkManager.Begin(true);
      var granted = await _permissionManager.GetAsync(eventData.Permissions.ToArray(), eventData.ProviderName, eventData.ProviderKey);
      var result = granted.Result.ToDictionary(x => x.Name, x => x.IsGranted);
      response.PermissionGrants = result;
      response.IsSuccessful = true;
      response.ErrorMessage = string.Empty;
    }
    catch (Exception ex)
    {
      response.ErrorMessage = ex.Message;
      response.IsSuccessful = false;
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
      await _responseContext.RespondAsync(response);
    }
  }
}
