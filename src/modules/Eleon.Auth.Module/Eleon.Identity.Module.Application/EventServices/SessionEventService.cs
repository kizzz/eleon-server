using Common.EventBus.Module;
using Commons.Module.Messages.Session;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Identity.Module.Sessions;

namespace VPortal.Identity.Module.EventServices.Sessions;

public class SessionEventService : ITransientDependency,
    IDistributedEventHandler<GetSessionByIdRequestMsg>,
    IDistributedEventHandler<GetSessionsForUserRequestMsg>,
    IDistributedEventHandler<RevokeSessionByIdRequestMsg>,
    IDistributedEventHandler<RevokeSessionsForUserRequestMsg>
{
  private readonly IVportalLogger<SessionEventService> _logger;
  private readonly IResponseContext _responseContext;
  private readonly SessionManager _sessionManager;
  private readonly IDistributedEventBus _eventBus;
  private readonly IUnitOfWorkManager _unitOfWorkManager;

  public SessionEventService(
      IVportalLogger<SessionEventService> logger,
      IResponseContext responseContext,
      SessionManager sessionManager,
      IDistributedEventBus eventBus,
      IUnitOfWorkManager unitOfWorkManager)
  {
    _logger = logger;
    _responseContext = responseContext;
    _sessionManager = sessionManager;
    _eventBus = eventBus;
    _unitOfWorkManager = unitOfWorkManager;
  }

  public async Task HandleEventAsync(GetSessionByIdRequestMsg eventData)
  {
    try
    {
      using var uow = _unitOfWorkManager.Begin(true);
      var session = await _sessionManager.GetByIdAsync(eventData.SessionId);
      await _responseContext
          .RespondAsync(new SessionResponseMsg
          {
            Session = session,
          });

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task HandleEventAsync(GetSessionsForUserRequestMsg eventData)
  {
    try
    {
      using var uow = _unitOfWorkManager.Begin(true);
      var sessions = await _sessionManager.GetForUserAsync(eventData.UserId);
      await _responseContext
          .RespondAsync(new SessionsResponseMsg
          {
            Sessions = sessions.ToList(),
          });

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task HandleEventAsync(RevokeSessionByIdRequestMsg eventData)
  {
    try
    {
      using var uow = _unitOfWorkManager.Begin(true);
      var session = await _sessionManager.GetByIdAsync(eventData.SessionId);
      await _sessionManager.RevokeAsync(eventData.SessionId);

      if (Guid.TryParse(session.UserId, out var userId))
      {
        await _eventBus.PublishAsync(
            new UserSessionsRevokedMsg
            {
              SessionId = eventData.SessionId,
              UserId = userId,
            });
      }

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task HandleEventAsync(RevokeSessionsForUserRequestMsg eventData)
  {
    try
    {
      using var uow = _unitOfWorkManager.Begin(true);
      await _sessionManager.RevokeAllAsync(eventData.UserId);

      if (Guid.TryParse(eventData.UserId, out var userId))
      {
        await _eventBus.PublishAsync(
            new UserSessionsRevokedMsg
            {
              UserId = userId,
            });
      }

      await uow.SaveChangesAsync();
      await uow.CompleteAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
