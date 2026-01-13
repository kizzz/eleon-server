using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.TenantManagement.Module.ClientIsolation;
using VPortal.TenantManagement.Module.TenantIsolation;

namespace VPortal.TenantManagement.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/TenantManagement/ClientIsolation")]
  public class ClientIsolationController : TenantManagementController, IClientIsolationAppService
  {
    private readonly IVportalLogger<ClientIsolationController> logger;
    private readonly IClientIsolationAppService clientIsolationAppService;

    public ClientIsolationController(
        IVportalLogger<ClientIsolationController> logger,
        IClientIsolationAppService clientIsolationAppService)
    {
      this.logger = logger;
      this.clientIsolationAppService = clientIsolationAppService;
    }

    [HttpGet("GetUserIsolationSettings")]
    public async Task<UserIsolationSettingsDto> GetUserIsolationSettings(Guid userId)
    {

      var response = await clientIsolationAppService.GetUserIsolationSettings(userId);


      return response;
    }

    [HttpPost("SetTenantIpIsolationSettings")]
    public async Task<bool> SetTenantIpIsolationSettings(SetIpIsolationRequestDto request)
    {

      var response = await clientIsolationAppService.SetTenantIpIsolationSettings(request);


      return response;
    }

    [HttpPost("SetTenantIsolation")]
    public async Task<bool> SetTenantIsolation(SetTenantIsolationRequestDto request)
    {

      var response = await clientIsolationAppService.SetTenantIsolation(request);


      return response;
    }

    [HttpPost("SetUserIsolation")]
    public async Task<bool> SetUserIsolation(SetUserIsolationRequestDto request)
    {

      var response = await clientIsolationAppService.SetUserIsolation(request);


      return response;
    }

    [HttpPost("ValidateClientIsolation")]
    public async Task<bool> ValidateClientIsolation(ValidateClientIsolationDto validateClientIsolationDto)
    {

      var response = await clientIsolationAppService.ValidateClientIsolation(validateClientIsolationDto);


      return response;
    }
  }
}
