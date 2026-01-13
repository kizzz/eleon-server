using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.Microservices;
using VPortal.SitesManagement.Module.UiModules;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/CoreInfrastructure/UiModules")]
  public class UiModuleController : ControllerBase, IUiModuleAppService
  {
    private readonly IUiModuleAppService _appService;
    private readonly IVportalLogger<UiModuleController> _logger;

    public UiModuleController(IUiModuleAppService appService, IVportalLogger<UiModuleController> logger)
    {
      _appService = appService;
      _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<EleoncoreModuleDto> GetAsync(Guid id)
    {

      var response = await _appService.GetAsync(id);


      return response;
    }

    [HttpGet("GetAllAsync")]
    public async Task<List<EleoncoreModuleDto>> GetAllAsync()
    {

      var response = await _appService.GetAllAsync();


      return response;
    }

    [HttpPost("CreateAsync")]
    public async Task<EleoncoreModuleDto> CreateAsync([FromBody] EleoncoreModuleDto input)
    {

      var response = await _appService.CreateAsync(input);


      return response;
    }

    [HttpPut("{id}")]
    public async Task<EleoncoreModuleDto> UpdateAsync(Guid id, [FromBody] EleoncoreModuleDto input)
    {

      var response = await _appService.UpdateAsync(id, input);


      return response;
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {

      await _appService.DeleteAsync(id);

    }

    [HttpGet("application/{applicationId}")]
    public async Task<List<EleoncoreModuleDto>> GetModulesByApplicationAsync(Guid applicationId)
    {

      var response = await _appService.GetModulesByApplicationAsync(applicationId);


      return response;
    }

    [HttpGet("enabled-modules")]
    public async Task<List<EleoncoreModuleDto>> GetEnabledModulesAsync()
    {

      var response = await _appService.GetEnabledModulesAsync();


      return response;
    }

  }
}


