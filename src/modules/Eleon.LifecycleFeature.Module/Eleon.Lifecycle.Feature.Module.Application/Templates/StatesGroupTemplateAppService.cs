using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using VPortal.Lifecycle.Feature.Module.DomainServices;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Permissions;

namespace VPortal.Lifecycle.Feature.Module.Templates
{
  [Authorize($"{LifecyclePermissions.General}")]
  public class StatesGroupTemplateAppService : ModuleAppService, IStatesGroupTemplateAppService
  {
    private readonly IVportalLogger<StatesGroupTemplateAppService> logger;
    private readonly StatesGroupTemplateDomainService statesGroupTemplateDomain;
    private readonly StateTemplateDomainService stateTemplateDomain;

    public StatesGroupTemplateAppService(
        IVportalLogger<StatesGroupTemplateAppService> logger,
        StatesGroupTemplateDomainService statesGroupTemplateDomain,
        StateTemplateDomainService stateTemplateDomain)
    {
      this.logger = logger;
      this.statesGroupTemplateDomain = statesGroupTemplateDomain;
      this.stateTemplateDomain = stateTemplateDomain;
    }

    public async Task<PagedResultDto<StatesGroupTemplateDto>> GetListAsync(GetStatesGroupsDto input)
    {
      PagedResultDto<StatesGroupTemplateDto> result = null;
      try
      {
        var pair = await statesGroupTemplateDomain
            .GetListAsync(input.DocumentObjectType, input.Sorting, input.MaxResultCount, input.SkipCount);
        var dtos = ObjectMapper
            .Map<List<StatesGroupTemplateEntity>, List<StatesGroupTemplateDto>>(pair.Value);
        result = new PagedResultDto<StatesGroupTemplateDto>(pair.Key, dtos);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Add(StatesGroupTemplateDto statesGroupTemplateDto)
    {
      bool result = false;
      try
      {
        var statesGroupTemplate = ObjectMapper
            .Map<StatesGroupTemplateDto, StatesGroupTemplateEntity>(statesGroupTemplateDto);
        result = await statesGroupTemplateDomain.Add(statesGroupTemplate);
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
        result = await statesGroupTemplateDomain.Remove(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Enable(StatesGroupSwitchDto groupEnableDto)
    {
      bool result = false;
      try
      {
        result = await statesGroupTemplateDomain.Enable(groupEnableDto.Id, groupEnableDto.newState);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Rename(Guid id, string newName)
    {
      bool result = false;
      try
      {
        result = await statesGroupTemplateDomain.Rename(id, newName);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    [Authorize(LifecyclePermissions.LifecycleManager)]
    public async Task<bool> Update(StatesGroupTemplateDto statesGroupTemplateDto)
    {
      bool result = false;
      try
      {
        var statesGroupTemplate = ObjectMapper
            .Map<StatesGroupTemplateDto, StatesGroupTemplateEntity>(statesGroupTemplateDto);

        result = await statesGroupTemplateDomain.Update(statesGroupTemplate);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public async Task<FullStatesGroupTemplateDto> GetAsync(Guid id)
    {
      FullStatesGroupTemplateDto result = null;
      try
      {
        var statesGroupTemplate = await statesGroupTemplateDomain.GetAsync(id);
        result = ObjectMapper
            .Map<StatesGroupTemplateEntity, FullStatesGroupTemplateDto>(statesGroupTemplate);

      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      return result;
    }
  }
}
