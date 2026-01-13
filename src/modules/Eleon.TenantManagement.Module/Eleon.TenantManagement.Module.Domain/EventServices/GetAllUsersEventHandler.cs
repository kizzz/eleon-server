using Common.EventBus.Module;
using EleonsoftSdk.Messages.User;
using Logging.Module;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.TenantManagement.Module.DomainServices;

namespace ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.EventServices;
internal class GetAllUsersEventHandler : IDistributedEventHandler<GetAllUsersRequestMsg>, ITransientDependency
{
  private readonly UserDomainService _userDomainService;
  private readonly IVportalLogger<GetAllUsersEventHandler> _logger;
  private readonly IResponseContext _responseContext;
  private readonly IUnitOfWorkManager unitOfWorkManager;

  public GetAllUsersEventHandler(
      UserDomainService userDomainService,
      IVportalLogger<GetAllUsersEventHandler> logger,
      IResponseContext responseContext,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _userDomainService = userDomainService;
    _logger = logger;
    _responseContext = responseContext;
    this.unitOfWorkManager = unitOfWorkManager;
  }
  public async Task HandleEventAsync(GetAllUsersRequestMsg eventData)
  {
    try
    {
      using var uow = unitOfWorkManager.Begin(true);
      var users = await _userDomainService.GetAllUsersListAsync();
      var response = new GetAllUsersResponseMsg
      {
        Users = users.Select(u => new EleoncoreUserEto
        {
          Id = u.Id,
          UserName = u.UserName,
          Name = u.Name,
          Surname = u.Surname,
          Email = u.Email,
          PhoneNumber = u.PhoneNumber,
          IsActive = u.IsActive,
          TenantId = u.TenantId,
        }).ToList()
      };

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
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
