using EleonsoftModuleCollector.SystemLog.Module.SystemLog.Module.Application.Contracts.SystemLog;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using VPortal.DocMessageLog.Module.Domain;
using VPortal.DocMessageLog.Module.Entities;
using VPortal.DocMessageLog.Module.Permissions;

namespace VPortal.DocMessageLog.Module.DocMessageLogs;

[Authorize(SystemLogModulePermissions.General)]
public class SystemLogAppService : SystemLogModuleAppService, ISystemLogAppService
{
  private readonly IVportalLogger<SystemLogAppService> _logger;
  private readonly SystemLogDomainService _domainService;

  public SystemLogAppService(
      IVportalLogger<SystemLogAppService> logger,
      SystemLogDomainService docMsgLogDomainService)
  {
    _logger = logger;
    _domainService = docMsgLogDomainService;
  }


  public async Task<FullSystemLogDto> GetByIdAsync(Guid id)
  {
    try
    {
      var entity = await _domainService.GetByIdAsync(id);
      var response = ObjectMapper.Map<SystemLogEntity, FullSystemLogDto>(entity);
      return response;
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

  public async Task<PagedResultDto<SystemLogDto>> GetListAsync(SystemLogListRequestDto input)
  {
    PagedResultDto<SystemLogDto> result = null;
    try
    {
      var pair = await _domainService.GetListAsync(
          input.Sorting,
          input.MaxResultCount,
          input.SkipCount,
          input.SearchQuery,
          input.MinLogLevelFilter,
          input.InitiatorFilter,
          input.InitiatorTypeFilter,
          input.ApplicationNameFilter,
          input.CreationFromDateFilter,
          input.CreationToDateFilter,
          input.OnlyUnread);

      var dtos = ObjectMapper.Map<List<SystemLogEntity>, List<SystemLogDto>>(pair.Value);
      result = new PagedResultDto<SystemLogDto>(pair.Key, dtos);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }

    return result;
  }

  public async Task<UnresolvedSystemLogCountDto> GetTotalUnresolvedCountAsync()
  {
    UnresolvedSystemLogCountDto result = null;
    try
    {
      var entity = await _domainService.GetTotalUnresolvedCount();
      result = ObjectMapper.Map<UnresolvedSystemLogCount, UnresolvedSystemLogCountDto>(entity);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
    return result;
  }
  public async Task MarkAllReadedAsync()
  {
    try
    {
      await _domainService.MarkAllReadedAsync();
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }
    finally
    {
    }
  }

  public async Task<bool> MarkReadedAsync(MarkSystemLogsReadedRequestDto markReadedRequestDto)
  {

    try
    {
      foreach (var logId in markReadedRequestDto.LogIds)
      {
        await _domainService.MarkReadedAsync(logId, markReadedRequestDto.IsReaded);
      }
      return true;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }

  public async Task<bool> WriteAsync(CreateSystemLogDto request)
  {

    try
    {

      var entity = new SystemLogEntity(GuidGenerator.Create())
      {
        ApplicationName = request.ApplicationName,
        Message = request.Message,
        LogLevel = request.LogLevel,
        InitiatorId = request.ExtraProperties.GetOrDefault("InitiatorId"),
        InitiatorType = request.ExtraProperties.GetOrDefault("InitiatorType")
      };

      foreach (var prop in request.ExtraProperties)
      {
        entity.SetProperty(prop.Key, prop.Value);
      }

      await _domainService.WriteAsync(entity);
      return true;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }

  public async Task<bool> WriteManyAsync(List<CreateSystemLogDto> request)
  {

    try
    {
      var entities = new List<SystemLogEntity>();
      foreach (var dto in request)
      {
        var entity = new SystemLogEntity(GuidGenerator.Create())
        {
          ApplicationName = dto.ApplicationName,
          Message = dto.Message,
          LogLevel = dto.LogLevel,
          InitiatorId = dto.ExtraProperties.GetOrDefault("InitiatorId"),
          InitiatorType = dto.ExtraProperties.GetOrDefault("InitiatorType")
        };

        foreach (var prop in dto.ExtraProperties)
        {
          entity.SetProperty(prop.Key, prop.Value);
        }

        entities.Add(entity);
      }

      var result = await _domainService.WriteManyAsync(entities);
      return result;
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }
}
