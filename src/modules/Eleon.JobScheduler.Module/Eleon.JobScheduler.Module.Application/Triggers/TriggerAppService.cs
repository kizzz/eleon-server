using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Triggers;
using JobScheduler.Module.Triggers;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Permissions;

namespace VPortal.JobScheduler.Module.Triggers
{
  [Authorize(JobSchedulerPermissions.General)]
  public class TriggerAppService : JobSchedulerModuleAppService, ITriggerAppService
  {
    private readonly IVportalLogger<TriggerAppService> logger;
    private readonly TriggerDomainService domainService;

    public TriggerAppService(
        IVportalLogger<TriggerAppService> logger,
        TriggerDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }
    public async Task<TriggerDto> AddAsync(TriggerDto trigger)
    {
      try
      {
        var toAdd = ObjectMapper.Map<TriggerDto, TriggerEntity>(trigger);
        var entity = await domainService.AddAsync(toAdd);
        return ObjectMapper.Map<TriggerEntity, TriggerDto>(entity);
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

    public async Task<bool> SetTriggerIsEnabledAsync(Guid triggerId, bool isEnabled)
    {
      bool response = false;
      try
      {
        response = await domainService.SetTriggerIsEnabled(triggerId, isEnabled);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<TriggerDto> UpdateAsync(TriggerDto trigger)
    {
      try
      {
        var entity = ObjectMapper.Map<TriggerDto, TriggerEntity>(trigger);
        var result = await domainService.UpdateAsync(entity);
        return ObjectMapper.Map<TriggerEntity, TriggerDto>(result);
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

    public async Task<TriggerDto> GetByIdAsync(Guid id)
    {
      TriggerDto response = null;
      try
      {
        var gotEntity = await domainService.GetByIdAsync(id);
        response = ObjectMapper.Map<TriggerEntity, TriggerDto>(gotEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<List<TriggerDto>> GetListAsync(TriggerListRequestDto request)
    {
      var response = new List<TriggerDto>();
      try
      {
        var gotEntities = await domainService.GetListAsync(request.TaskId, request.IsEnabledFilter);
        response = ObjectMapper.Map<List<TriggerEntity>, List<TriggerDto>>(gotEntities);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {

      try
      {
        await domainService.DeleteAsync(id);
        return true;
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
}
