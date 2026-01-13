using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Permissions;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  [Authorize($"{LifecyclePermissions.General}")]
  public class StateTemplateAppService : ModuleAppService, IStateTemplateAppService
  {
    private readonly IVportalLogger<StateTemplateAppService> logger;
    private readonly StateTemplateDomainService stateTemplateDomain;

    public StateTemplateAppService(
        IVportalLogger<StateTemplateAppService> logger,
        StateTemplateDomainService stateTemplateDomain)
    {
      this.logger = logger;
      this.stateTemplateDomain = stateTemplateDomain;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Add(StateTemplateDto stateTemplateDto)
    {
      bool result = false;
      try
      {
        var stateTemplate = ObjectMapper
            .Map<StateTemplateDto, StateTemplateEntity>(stateTemplateDto);
        result = await stateTemplateDomain.Add(stateTemplate);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Enable(StateSwitchDto input)
    {
      bool result = false;
      try
      {
        result = await stateTemplateDomain.Enable(input.Id, input.NewState);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<List<StateTemplateDto>> GetAllAsync(Guid groupId)
    {
      List<StateTemplateDto> result = new();
      try
      {
        var states = await stateTemplateDomain.GetAllAsync(groupId);
        result = ObjectMapper
            .Map<List<StateTemplateEntity>, List<StateTemplateDto>>(states);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Remove(Guid groupId, Guid stateId)
    {
      bool result = false;
      try
      {
        result = await stateTemplateDomain.Remove(groupId, stateId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> UpdateApprovalType(UpdateApprovalTypeDto update)
    {
      bool result = false;
      try
      {
        result = await stateTemplateDomain.UpdateApprovalType(update.Id, update.NewApprovalType);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> UpdateName(Guid id, string name)
    {

      bool result;
      try
      {
        result = await stateTemplateDomain.UpdateName(id, name);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
      return result;

    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> UpdateOrderIndex(UpdateOrderIndexDto update)
    {
      bool result = new();
      try
      {
        result = await stateTemplateDomain.UpdateOrderIndex(update.Id, update.OrderIndex);
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
      bool result = new();
      try
      {
        result = await stateTemplateDomain.UpdateOrderIndexes(order);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }
  }
}
