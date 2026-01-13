using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Permissions;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  [Authorize($"{LifecyclePermissions.General}")]
  public class StateActorTemplateAppService : ModuleAppService, IStateActorTemplateAppService
  {
    private readonly IVportalLogger<StateActorTemplateAppService> logger;
    private readonly StateActorTemplateDomainService stateActorTemplateDomain;

    public StateActorTemplateAppService(
        IVportalLogger<StateActorTemplateAppService> logger,
        StateActorTemplateDomainService stateActorTemplateDomain)
    {
      this.logger = logger;
      this.stateActorTemplateDomain = stateActorTemplateDomain;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Add(StateActorTemplateDto stateActorTemplateDto)
    {
      bool result = false;
      try
      {
        var stateActorTemplate = ObjectMapper
            .Map<StateActorTemplateDto, StateActorTemplateEntity>(stateActorTemplateDto);
        result = await stateActorTemplateDomain.Add(stateActorTemplate);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Enable(StateSwitchDto stateSwitchDto)
    {
      bool result = false;
      try
      {
        result = await stateActorTemplateDomain.Enable(stateSwitchDto.Id, stateSwitchDto.NewState);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<StateActorTemplateDto>> GetAllAsync(Guid stateId)
    {
      List<StateActorTemplateDto> result = new();
      try
      {
        var list = await stateActorTemplateDomain.GetAllAsync(stateId);
        result =
            ObjectMapper
            .Map<
                List<StateActorTemplateEntity>,
                List<StateActorTemplateDto>
             >(list);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Remove(Guid id)
    {
      bool result = false;
      try
      {
        result = await stateActorTemplateDomain.Remove(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Update(StateActorTemplateDto stateActorTemplateDto)
    {
      bool result = false;
      try
      {
        var stateActorTemplate = ObjectMapper
            .Map<StateActorTemplateDto, StateActorTemplateEntity>(stateActorTemplateDto);
        result = await stateActorTemplateDomain.Update(stateActorTemplate);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> UpdateOrderIndexes(Dictionary<Guid, int> order)
    {
      bool result = false;
      try
      {
        result = await stateActorTemplateDomain.UpdateOrderIndexes(order);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
