using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Application.Contracts.Actions;
using Logging.Module;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VPortal.JobScheduler.Module.DomainServices;
using VPortal.JobScheduler.Module.Entities;

namespace VPortal.JobScheduler.Module.Actions
{
  public class ActionAppService : JobSchedulerModuleAppService, IActionAppService
  {
    private readonly IVportalLogger<ActionAppService> logger;
    private readonly ActionDomainService domainService;

    public ActionAppService(
        IVportalLogger<ActionAppService> logger,
        ActionDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<ActionDto> AddAsync(ActionDto action)
    {

      try
      {
        var entity = ObjectMapper.Map<ActionDto, ActionEntity>(action);
        var result = await domainService.AddAsync(
            action.TaskId,
            entity
            );
        return ObjectMapper.Map<ActionEntity, ActionDto>(result);
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

    public async Task<ActionDto> GetByIdAsync(Guid id)
    {
      try
      {
        var entity = await domainService.GetByIdAsync(id);
        return ObjectMapper.Map<ActionEntity, ActionDto>(entity);
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

    public async Task<List<ActionDto>> GetListAsync(ActionListRequestDto request)
    {
      try
      {
        var result = await domainService.GetListAsync(request.TaskId, request.NameFilter, request.Sorting);
        return ObjectMapper.Map<List<ActionEntity>, List<ActionDto>>(result.Value);
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

    public async Task<ActionDto> UpdateAsync(ActionDto action)
    {
      try
      {
        var entity = ObjectMapper.Map<ActionDto, ActionEntity>(action);
        var result = await domainService.UpdateAsync(entity);
        return ObjectMapper.Map<ActionEntity, ActionDto>(result);
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

    public Task<bool> DeleteAsync(Guid id)
    {
      try
      {
        return domainService.DeleteAsync(id);
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
  }
}
