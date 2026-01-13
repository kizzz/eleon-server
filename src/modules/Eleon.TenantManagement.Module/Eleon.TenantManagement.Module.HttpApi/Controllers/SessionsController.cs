using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using VPortal.Identity.Module.Sessions;

namespace VPortal.TenantManagement.Module.Controllers;

[Area(TenantManagementRemoteServiceConsts.ModuleName)]
[RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
[Route("api/TenantManagement/Sessions")]
public class SessionsController : TenantManagementController, ISessionAppService
{
  private readonly IVportalLogger<SessionsController> logger;
  private readonly ISessionAppService sessionService;

  public SessionsController(IVportalLogger<SessionsController> logger, ISessionAppService sessionService)
  {
    this.logger = logger;
    this.sessionService = sessionService;
  }

  [HttpGet("GetById")]
  public async Task<UserSessionDto> GetByIdAsync(string sessionId)
  {

    var response = await sessionService.GetByIdAsync(sessionId);


    return response;
  }

  [HttpGet("GetCurrentSession")]
  public async Task<UserSessionDto> GetCurrentSessionAsync()
  {

    var response = await sessionService.GetCurrentSessionAsync();


    return response;
  }

  [HttpGet("GetForCurrentUser")]
  public async Task<IReadOnlyList<UserSessionDto>> GetForCurrentUserAsync()
  {
    // todo for current user
    var response = await sessionService.GetForCurrentUserAsync();


    return response;
  }

  [HttpGet("GetForUser")]
  public async Task<IReadOnlyList<UserSessionDto>> GetForUserAsync(string userId)
  {

    var response = await sessionService.GetForUserAsync(userId);


    return response;
  }

  [HttpPost("RevokeAll")]
  public async Task RevokeAllAsync()
  {

    await sessionService.RevokeAllAsync();

  }

  [HttpDelete("Revoke")]
  public async Task RevokeAsync(string sessionId)
  {

    await sessionService.RevokeAsync(sessionId);

  }

  [HttpPost("RevokeCurrent")]
  public async Task RevokeCurrentAsync()
  {

    await sessionService.RevokeCurrentAsync();

  }
}
