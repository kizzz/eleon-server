using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.TenantManagement.Module.ControlDelegations;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/ControlDelegation")]
  public class ControlDelegationController : TenantManagementController, IControlDelegationAppService
  {
    private readonly IVportalLogger<ControlDelegationController> logger;
    private readonly IControlDelegationAppService controlDelegationAppService;

    public ControlDelegationController(
        IVportalLogger<ControlDelegationController> logger,
        IControlDelegationAppService controlDelegationAppService)
    {
      this.logger = logger;
      this.controlDelegationAppService = controlDelegationAppService;
    }

    [HttpPost("AddControlDelegation")]
    public async Task<bool> AddControlDelegation(CreateControlDelegationRequestDto request)
    {

      var response = await controlDelegationAppService.AddControlDelegation(request);


      return response;
    }

    [HttpGet("GetActiveControlDelegationsByUser")]
    public async Task<List<ControlDelegationDto>> GetActiveControlDelegationsByUser()
    {

      var response = await controlDelegationAppService.GetActiveControlDelegationsByUser();


      return response;
    }

    [HttpGet("GetActiveControlDelegationsToUser")]
    public async Task<List<ControlDelegationDto>> GetActiveControlDelegationsToUser()
    {

      var response = await controlDelegationAppService.GetActiveControlDelegationsToUser();


      return response;
    }

    [HttpGet("GetControlDelegation")]
    public async Task<ControlDelegationDto> GetControlDelegation(Guid delegationId)
    {

      var response = await controlDelegationAppService.GetControlDelegation(delegationId);


      return response;
    }

    [HttpGet("GetControlDelegationsByUser")]
    public async Task<PagedResultDto<ControlDelegationDto>> GetControlDelegationsByUser(int skip, int take)
    {

      var response = await controlDelegationAppService.GetControlDelegationsByUser(skip, take);


      return response;
    }

    [HttpPost("SetControlDelegationActiveState")]
    public async Task<bool> SetControlDelegationActiveState(Guid delegationId, bool isActive)
    {

      var response = await controlDelegationAppService.SetControlDelegationActiveState(delegationId, isActive);


      return response;
    }

    [HttpPost("UpdateControlDelegation")]
    public async Task<bool> UpdateControlDelegation(UpdateControlDelegationRequestDto request)
    {

      var response = await controlDelegationAppService.UpdateControlDelegation(request);


      return response;
    }
  }
}
