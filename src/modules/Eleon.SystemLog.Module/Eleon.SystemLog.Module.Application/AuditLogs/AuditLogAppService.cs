using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AuditLogging;
using VPortal.DocMessageLog.Module;
using VPortal.Infrastructure.Module.Domain.DomainServices;

namespace VPortal.Infrastructure.Module.AuditLogs
{
  public class AuditLogAppService : SystemLogModuleAppService, IAuditLogAppService
  {
    private readonly IVportalLogger<AuditLogAppService> logger;
    private readonly AuditLogDomainService domainService;

    public AuditLogAppService(
        IVportalLogger<AuditLogAppService> logger,
        AuditLogDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<AuditLogDto> GetAuditLogById(Guid id)
    {
      AuditLogDto result = null;
      try
      {
        var entity = await domainService.GetAuditLogById(id);
        result = ObjectMapper.Map<AuditLog, AuditLogDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      return result;
    }

    public async Task<PagedResultDto<AuditLogHeaderDto>> GetAuditLogList(AuditLogListRequestDto input)
    {
      PagedResultDto<AuditLogHeaderDto> result = new PagedResultDto<AuditLogHeaderDto>();
      try
      {
        var pair = await domainService.GetAuditList(input.Sorting,
                                                    input.MaxResultCount,
                                                    input.SkipCount,
                                                    input.StartTime,
                                                    input.EndTime,
                                                    input.HttpMethod,
                                                    input.Url,
                                                    input.UserId,
                                                    input.UserName,
                                                    input.ApplicationName,
                                                    input.ClientIpAddress,
                                                    input.CorrelationId,
                                                    input.MaxExecutionDuration,
                                                    input.MinExecutionDuration,
                                                    input.HasException,
                                                    input.HttpStatusCode);
        var dtos = ObjectMapper
            .Map<List<AuditLog>, List<AuditLogHeaderDto>>(pair.Value);
        result = new PagedResultDto<AuditLogHeaderDto>(pair.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<EntityChangeDto> GetEntityChangeById(Guid id)
    {
      EntityChangeDto result = null;
      try
      {
        var entity = await domainService.GetEntityChangeById(id);
        result = ObjectMapper.Map<EntityChange, EntityChangeDto>(entity);

        var actorMap = await domainService.GetActorUsersByAuditLogIdsAsync(new[] { result.AuditLogId });
        if (actorMap.TryGetValue(result.AuditLogId, out var user))
        {
          result.UpdatedById = user.userId;
          result.UpdatedByName = user.userName;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<PagedResultDto<EntityChangeDto>> GetEntityChangeList(EntityChangeListRequestDto input)
    {
      var result = new PagedResultDto<EntityChangeDto>();

      try
      {
        var pair = await domainService.GetEntityChangeList(
            input.Sorting, input.MaxResultCount, input.SkipCount,
            input.AuditLogId, input.StartTime, input.EndTime,
            input.EntityChangeType, input.EntityId, input.EntityTypeFullName);

        var dtos = ObjectMapper.Map<List<EntityChange>, List<EntityChangeDto>>(pair.Value);

        var auditLogIds = dtos.Select(x => x.AuditLogId).Distinct().ToList();
        var actorMap = await domainService.GetActorUsersByAuditLogIdsAsync(auditLogIds);

        foreach (var dto in dtos)
        {
          if (actorMap.TryGetValue(dto.AuditLogId, out var user))
          {
            dto.UpdatedById = user.userId;
            dto.UpdatedByName = user.userName;
          }
        }

        result = new PagedResultDto<EntityChangeDto>(pair.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> AddAuditAsync(AuditLogDto input)
    {
      try
      {
        var entity = ObjectMapper.Map<AuditLogDto, AuditLog>(input);
        await domainService.AddAuditLogAsync(entity);

        return true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        return false;
      }
      finally
      {
      }
    }
  }
}
