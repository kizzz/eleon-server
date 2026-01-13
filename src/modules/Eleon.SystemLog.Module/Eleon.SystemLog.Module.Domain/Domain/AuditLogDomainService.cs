using EleonsoftAbp.Auth;
using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Helpers;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{

  public class AuditLogDomainService : DomainService, ISingletonDependency
  {

    private readonly IAuditLogRepository auditLogRepository;
    private readonly IVportalLogger<AuditLogDomainService> logger;
    private readonly ICurrentUser _currentUser;
    private readonly IConfiguration _configuration;

    public AuditLogDomainService(
        IAuditLogRepository auditLogRepository,
        IVportalLogger<AuditLogDomainService> logger,
        ICurrentUser currentUser,
        IConfiguration configuration)
    {
      this.auditLogRepository = auditLogRepository;
      this.logger = logger;
      _currentUser = currentUser;
      _configuration = configuration;
    }

    public async Task<KeyValuePair<long, List<AuditLog>>> GetAuditList(
       string sorting = null,
       int maxResultCount = int.MaxValue,
       int skipCount = 0,
       DateTime? startTime = null,
       DateTime? endTime = null,
       string httpMethod = null,
       string url = null,
       Guid? userId = null,
       string userName = null,
       string applicationName = null,
       string clientIpAddress = null,
       string correlationId = null,
       int? maxExecutionDuration = null,
       int? minExecutionDuration = null,
       bool? hasException = null,
       HttpStatusCode? httpStatusCode = null)
    {
      KeyValuePair<long, List<AuditLog>> result = default;
      long count = 0;
      List<AuditLog> auditLogs = new List<AuditLog>();
      try
      {
        if (startTime.HasValue)
        {
          startTime = startTime.Value.AddDays(1).Date.Date;
        }

        if (endTime.HasValue)
        {
          endTime = endTime.Value.Date.AddDays(1).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        count = await auditLogRepository.GetCountAsync(startTime, endTime, httpMethod, url, null, userId,
                                                userName, applicationName, clientIpAddress, correlationId,
                                                maxExecutionDuration, minExecutionDuration, hasException, httpStatusCode);
        if (count > 0)
        {
          auditLogs = await auditLogRepository.GetListAsync(sorting, maxResultCount, skipCount, startTime, endTime, httpMethod,
                                                              url, null, userId, userName, applicationName, clientIpAddress, correlationId,
                                                              maxExecutionDuration, minExecutionDuration,
                                                              hasException, httpStatusCode);
        }

        result = new KeyValuePair<long, List<AuditLog>>(count, auditLogs);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<AuditLog> GetAuditLogById(Guid id)
    {
      AuditLog result = null;
      try
      {
        result = await auditLogRepository.GetAsync(id, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    public async Task<KeyValuePair<long, List<EntityChange>>> GetEntityChangeList(
       string sorting = null,
       int maxResultCount = int.MaxValue,
       int skipCount = 0,
       Guid? auditLogId = null,
       DateTime? startTime = null,
       DateTime? endTime = null,
       EntityChangeType? entityChangeType = null,
       string entityId = null,
       string entityTypeFullName = null)
    {
      KeyValuePair<long, List<EntityChange>> result = default;
      long count = 0;
      List<EntityChange> list = new List<EntityChange>();
      try
      {
        if (startTime.HasValue)
        {
          startTime = startTime.Value.AddDays(1).Date.Date;
        }

        if (endTime.HasValue)
        {
          endTime = endTime.Value.Date.AddDays(1).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        count = await auditLogRepository.GetEntityChangeCountAsync(auditLogId, startTime, endTime, entityChangeType, entityId, entityTypeFullName);
        if (count > 0)
        {
          list = await auditLogRepository.GetEntityChangeListAsync(sorting, maxResultCount, skipCount, auditLogId, startTime,
                                                                  endTime, entityChangeType, entityId, entityTypeFullName);
        }

        result = new KeyValuePair<long, List<EntityChange>>(count, list);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<EntityChange> GetEntityChangeById(Guid id)
    {
      EntityChange result = null;
      try
      {
        result = await auditLogRepository.GetEntityChange(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task AddAuditLogAsync(AuditLog audit)
    {
      try
      {
        var entity = AuditLogHelper.CopyWithNewIds(audit, GuidGenerator);
        await auditLogRepository.InsertAsync(entity, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }

    public async Task<Dictionary<Guid, (Guid? userId, string userName)>> GetActorUsersByAuditLogIdsAsync(IReadOnlyCollection<Guid> auditLogIds)
    {
      if (auditLogIds == null || auditLogIds.Count == 0)
        return new Dictionary<Guid, (Guid? userId, string userName)>();

      var query = await auditLogRepository.GetQueryableAsync();

      // "Actor" rule (recommended): if impersonation happened, prefer impersonator; else user
      return await query
          .Where(al => auditLogIds.Contains(al.Id))
          .Select(al => new { al.Id, al.UserId, al.ImpersonatorUserId, al.UserName, al.ImpersonatorUserName })
          .ToDictionaryAsync(
              x => x.Id,
              x => (x.ImpersonatorUserId ?? x.UserId, x.ImpersonatorUserName ?? x.UserName)
          );
    }
  }
}
