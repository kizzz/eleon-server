using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.ApplicationMenuItems;
using VPortal.SitesManagement.Module.Consts;

namespace VPortal.SitesManagement.Module.Controllers
{
  [Area(SitesManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = SitesManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/CoreInfrastructure/ApplicationMenuItems")]
  public class ApplicationMenuItemController : ControllerBase, IApplicationMenuItemAppService
  {
    private readonly IApplicationMenuItemAppService _appService;
    private readonly IVportalLogger<ApplicationMenuItemController> _logger;

    public ApplicationMenuItemController(IApplicationMenuItemAppService appService, IVportalLogger<ApplicationMenuItemController> logger)
    {
      _appService = appService;
      _logger = logger;
    }

    [HttpPost("Update")]
    public async Task<List<ApplicationMenuItemDto>> UpdateAsync(Guid applicationId, List<ApplicationMenuItemDto> itemsToUpdate)
    {

      var response = await _appService.UpdateAsync(applicationId, itemsToUpdate);


      return response;
    }

    [HttpGet("GetListByApplicationId")]
    public async Task<List<ApplicationMenuItemDto>> GetListByApplicationIdAsync(Guid applicationId)
    {

      var response = await _appService.GetListByApplicationIdAsync(applicationId);


      return response;
    }

    [HttpPost("GetListByApplicationIdAndMenuType")]
    public async Task<List<ApplicationMenuItemDto>> GetListByApplicationIdAndMenuTypeAsync(Guid applicationId, MenuType menuType)
    {

      var response = await _appService.GetListByApplicationIdAndMenuTypeAsync(applicationId, menuType);


      return response;
    }
  }
}


