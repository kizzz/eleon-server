using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.Grants;
using Volo.Abp.ObjectMapping;
using VPortal.Identity.Module.Entities;

namespace VPortal.Identity.Module.Sessions;
public class SessionManager : DomainService
{
  private readonly IVportalLogger<SessionManager> logger;
  private readonly IPersistentGrantRepository persistentGrantRepository;
  private readonly IdentitySecurityLogManager securityLogManger;
  private readonly IObjectMapper objectMapper;
  private readonly IdentityUserManager userManager;
  private readonly IHttpContextAccessor httpContextAccessor;

  public SessionManager(
      IVportalLogger<SessionManager> logger,
      IPersistentGrantRepository persistentGrantRepository,
      IdentitySecurityLogManager securityLogManger,
      IObjectMapper objectMapper,
      IdentityUserManager userManager,
      IHttpContextAccessor httpContextAccessor)
  {
    this.logger = logger;
    this.persistentGrantRepository = persistentGrantRepository;
    this.securityLogManger = securityLogManger;
    this.objectMapper = objectMapper;
    this.userManager = userManager;
    this.httpContextAccessor = httpContextAccessor;
  }

  private async Task<IQueryable<PersistedGrant>> GetSessionGrantsAsync()
  {
    return await persistentGrantRepository.GetDbSetAsync();
  }

  public Task<UserSessionEto> GetCurrentAsync()
  {

    try
    {
      var sessionId = ParseSessionHelper.GenerateSessionId(httpContextAccessor.HttpContext);
      return GetByIdAsync(sessionId);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<UserSessionEto> GetByIdAsync(string sessionId)
  {

    try
    {
      var grants = await (await GetSessionGrantsAsync())
          .Where(g => g.SessionId == sessionId)
          .ToListAsync();

      var grant = grants.FirstOrDefault();

      if (grant == null)
      {
        logger.Log.LogError("User session with id {sessionId} not found", sessionId);
        throw new EntityNotFoundException();
      }

      var session = objectMapper.Map<PersistedGrant, UserSessionEto>(grant);
      session.LastAccessTime = grants.Max(g => g.ConsumedTime ?? grant.CreationTime);
      return session;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<IReadOnlyList<UserSessionEto>> GetAllAsync()
  {

    try
    {
      var grants = await (await GetSessionGrantsAsync())
          .ToListAsync();

      return GetSessions(grants);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task<IReadOnlyList<UserSessionEto>> GetForUserAsync(string userId)
  {

    try
    {
      var grants = await (await GetSessionGrantsAsync())
          .Where(g => g.SubjectId == userId)
          .ToListAsync();

      return GetSessions(grants);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  private List<UserSessionEto> GetSessions(List<PersistedGrant> grants)
  {
    var sessions = grants
            .Where(g => !string.IsNullOrEmpty(g.SessionId))
            .DistinctBy(g => g.SessionId)
            .Select(objectMapper.Map<PersistedGrant, UserSessionEto>)
            .ToList();

    var groupedGrants = grants
        .Where(g => g.ConsumedTime.HasValue)
        .Where(g => !string.IsNullOrEmpty(g.SessionId))
        .GroupBy(g => g.SessionId)
        .ToDictionary(
            g => g.Key,
    g => g.Max(g => g.ConsumedTime ?? g.CreationTime)
    );

    sessions.ForEach(s =>
    {
      var lastAccess = groupedGrants.GetOrDefault(s.Id);
      s.LastAccessTime = lastAccess == default ? s.SignInDate : lastAccess;
    });

    return sessions;
  }


  public async Task RevokeAsync(string sessionId)
  {

    try
    {
      var grants = await (await GetSessionGrantsAsync())
          .Where(g => g.SessionId == sessionId)
          .ToListAsync();

      var existGrant = grants.FirstOrDefault();
      if (existGrant == null)
      {
        logger.Log.LogError("User session with id {sessionId} not found", sessionId);
        throw new EntityNotFoundException();
      }

      await persistentGrantRepository.DeleteManyAsync(grants);

      var user = await userManager.FindByIdAsync(existGrant.SubjectId);
      await securityLogManger.SaveAsync(new IdentitySecurityLogContext()
      {
        Identity = "IdentityServer",
        Action = "RevokeSession",
        UserName = user?.UserName
      });
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }

  public async Task RevokeAllAsync(string userId)
  {

    try
    {
      var grants = await (await GetSessionGrantsAsync())
          .Where(g => g.SubjectId == userId)
          .ToListAsync();

      await persistentGrantRepository.DeleteManyAsync(grants);
      var user = await userManager.FindByIdAsync(userId);
      await securityLogManger.SaveAsync(new IdentitySecurityLogContext()
      {
        Identity = "IdentityServer",
        Action = "RevokeAllSessions",
        UserName = user?.UserName
      });
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      throw;
    }
    finally
    {
    }
  }
}
