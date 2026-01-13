using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Templates;

namespace VPortal.Lifecycle.Feature.Module.Controllers.Templates
{
  [Area(ModuleRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = ModuleRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Lifecycle/Templates/StatesGroup")]
  public class StatesGroupTemplateController : ModuleController, IStatesGroupTemplateAppService
  {
    private readonly IStatesGroupTemplateAppService statesGroupTemplateAppService;
    private readonly IVportalLogger<StatesGroupTemplateController> _logger;

    public StatesGroupTemplateController(
        IStatesGroupTemplateAppService statesGroupTemplateAppService,
        IVportalLogger<StatesGroupTemplateController> logger)
    {
      this.statesGroupTemplateAppService = statesGroupTemplateAppService;
      _logger = logger;
    }

    [HttpGet("GetList")]
    public async Task<PagedResultDto<StatesGroupTemplateDto>> GetListAsync(GetStatesGroupsDto input)
    {

      var response = await statesGroupTemplateAppService.GetListAsync(input);


      return response;
    }

    [HttpPost("Add")]
    public async Task<bool> Add(StatesGroupTemplateDto statesGroupTemplate)
    {

      bool response = await statesGroupTemplateAppService.Add(statesGroupTemplate);


      return response;
    }

    [HttpDelete("Remove")]
    public async Task<bool> Remove(Guid id)
    {

      bool response = await statesGroupTemplateAppService.Remove(id);


      return response;
    }

    [HttpPut("Enable")]
    public async Task<bool> Enable(StatesGroupSwitchDto groupEnableDto)
    {

      bool response = await statesGroupTemplateAppService.Enable(groupEnableDto);


      return response;
    }

    [HttpPost("Rename")]
    public async Task<bool> Rename(Guid id, string newName)
    {

      bool response = await statesGroupTemplateAppService.Rename(id, newName);


      return response;
    }

    [HttpPost("Update")]
    public async Task<bool> Update(StatesGroupTemplateDto statesGroupTemplate)
    {

      bool response = await statesGroupTemplateAppService.Update(statesGroupTemplate);


      return response;
    }

    [HttpGet("Get")]
    public Task<FullStatesGroupTemplateDto> GetAsync(Guid id)
    {
      var response = statesGroupTemplateAppService.GetAsync(id);
      return response;
    }
  }
}
