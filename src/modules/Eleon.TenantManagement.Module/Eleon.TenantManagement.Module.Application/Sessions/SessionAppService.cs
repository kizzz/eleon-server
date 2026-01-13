using Common.EventBus.Module;
using Commons.Module.Messages.Session;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.EventServices.Sessions;
using VPortal.Identity.Module.Permissions;
using VPortal.Identity.Module.Sessions;
using VPortal.TenantManagement.Module;

namespace VPortal.Identity.Module.Application.Sessions;

[Authorize]
[ExposeServices(typeof(ISessionAppService))]
[Volo.Abp.DependencyInjection.Dependency(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient, ReplaceServices = true)]
public class SessionAppService : TenantManagementAppService, ISessionAppService
{
  private readonly IVportalLogger<SessionAppService> logger;
  private readonly ICurrentUser currentUser;
  private readonly IAuthorizationService authorizationService;
  private readonly IDistributedEventBus _eventBus;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public SessionAppService(
      IVportalLogger<SessionAppService> logger,
      ICurrentUser currentUser,
      IAuthorizationService authorizationService,
      IDistributedEventBus eventBus,
      IHttpContextAccessor httpContextAccessor)
  {
    this.logger = logger;
    this.currentUser = currentUser;
    this.authorizationService = authorizationService;
    _eventBus = eventBus;
    _httpContextAccessor = httpContextAccessor;
  }
  public async Task<UserSessionDto> GetByIdAsync(string sessionId)
  {

    try
    {
      var entity = (await _eventBus.RequestAsync<SessionResponseMsg>(new GetSessionByIdRequestMsg
      {
        SessionId = sessionId
      })).Session;
      return ObjectMapper.Map<UserSessionEto, UserSessionDto>(entity);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<UserSessionDto> GetCurrentSessionAsync()
  {

    try
    {
      var sessionId = ParseSessionHelper.GenerateSessionId(_httpContextAccessor.HttpContext);
      var entity = (await _eventBus.RequestAsync<SessionResponseMsg>(new GetSessionByIdRequestMsg
      {
        SessionId = sessionId
      })).Session;
      return ObjectMapper.Map<UserSessionEto, UserSessionDto>(entity);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task<IReadOnlyList<UserSessionDto>> GetForCurrentUserAsync()
  {

    try
    {
      var entity = (await _eventBus.RequestAsync<SessionsResponseMsg>(new GetSessionsForUserRequestMsg
      {
        UserId = currentUser.Id?.ToString()
      })).Sessions;
      return ObjectMapper.Map<IReadOnlyList<UserSessionEto>, IReadOnlyList<UserSessionDto>>(entity);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  [Authorize(IdentityPermissions.Session.Revoke)]
  public async Task<IReadOnlyList<UserSessionDto>> GetForUserAsync(string userId)
  {

    try
    {
      var entity = (await _eventBus.RequestAsync<SessionsResponseMsg>(new GetSessionsForUserRequestMsg
      {
        UserId = userId
      })).Sessions;
      return ObjectMapper.Map<IReadOnlyList<UserSessionEto>, IReadOnlyList<UserSessionDto>>(entity);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task RevokeAllAsync()
  {

    try
    {
      if (!CurrentUser.Id.HasValue)
      {
        throw new UnauthorizedAccessException("Current user is not authenticated.");
      }

      await _eventBus.PublishAsync(new RevokeSessionsForUserRequestMsg
      {
        UserId = currentUser.Id.Value.ToString()
      });
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task RevokeAsync(string sessionId)
  {

    try
    {
      var session = await GetByIdAsync(sessionId);

      if (session.UserId == currentUser.Id.ToString())
      {
        await PrivateSessionRevokeAsync(sessionId, session.UserId);
        return;
      }

      await authorizationService.CheckAsync(IdentityPermissions.Session.Revoke);

      await PrivateSessionRevokeAsync(sessionId, session.UserId);
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  public async Task RevokeCurrentAsync()
  {

    try
    {
      var session = await GetCurrentSessionAsync();
      await PrivateSessionRevokeAsync(session.Id, session.UserId);
    }
    catch (EntityNotFoundException notFound)
    {
      logger.Log.LogWarning(notFound, "Current session was not found");
    }
    catch (Exception e)
    {
      logger.Capture(e);
      throw;
    }
    finally
    {
    }
  }

  private async Task PrivateSessionRevokeAsync(string sessionId, string userIdStr)
  {
    await _eventBus.PublishAsync(new RevokeSessionByIdRequestMsg
    {
      SessionId = sessionId
    });
  }
}
