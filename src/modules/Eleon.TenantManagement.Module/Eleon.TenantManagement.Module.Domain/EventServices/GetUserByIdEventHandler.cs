using Common.EventBus.Module;
using EleonsoftSdk.Messages.User;
using Logging.Module;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.TenantManagement.Module.DomainServices;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.EventServices;
internal class GetUserByIdEventHandler : IDistributedEventHandler<GetUserByIdRequestMsg>, ITransientDependency
{
  private readonly UserDomainService _userDomainService;
  private readonly IVportalLogger<GetUserByIdEventHandler> _logger;
  private readonly IResponseContext _responseContext;

  public GetUserByIdEventHandler(
      UserDomainService userDomainService,
      IVportalLogger<GetUserByIdEventHandler> logger,
      IResponseContext responseContext)
  {
    _userDomainService = userDomainService;
    _logger = logger;
    _responseContext = responseContext;
  }
  public async Task HandleEventAsync(GetUserByIdRequestMsg eventData)
  {
    try
    {
      var user = await _userDomainService.GetById(eventData.UserId);
      var response = new GetUserByIdResponseMsg
      {
        User = new EleoncoreUserEto
        {
          Id = user.Id,
          TenantId = user.TenantId,
          UserName = user.UserName,
          Name = user.Name,
          Surname = user.Surname,
          Email = user.Email,
          PhoneNumber = user.PhoneNumber,
          IsActive = user.IsActive,
        }
      };
      await _responseContext.RespondAsync(response);
    }
    catch (Exception ex)
    {
      _logger.CaptureAndSuppress(ex);
    }
    finally
    {
    }
  }
}
