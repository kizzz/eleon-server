using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace VPortal.Accounting.Module.Controllers
{
  // ❌ DEPRECATED: AccountingModuleEntity has been deleted. This entire controller is deprecated.
  // TODO: Remove or refactor this controller after migration is complete.
  /*
  [Area(AccountingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AccountingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/account/accountModules")]
  public class AccountModuleController : ModuleController, IModuleAppService
  {
    private readonly IModuleAppService appService;
    private readonly IVportalLogger<AccountModuleController> logger;

    public AccountModuleController(
        IVportalLogger<AccountModuleController> logger,
        IModuleAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("GetAccountModuleList")]
    public async Task<PagedResultDto<ModuleDto>> GetAccountModuleList(ModuleListRequestDto input)
    {

      var response = await appService.GetAccountModuleList(input);


      return response;
    }

    [HttpGet("GetModuleById")]
    public async Task<ModuleDto> GetModuleById(Guid id)
    {

      var response = await appService.GetModuleById(id);


      return response;
    }

    [HttpPost("RemoveAccountModule")]
    public async Task<string> RemoveAccountModule(Guid id)
    {

      var response = await appService.RemoveAccountModule(id);


      return response;
    }

    [HttpPost("UpdateModule")]
    public async Task<ModuleDto> UpdateModule(ModuleDto updatedDto)
    {

      var response = await appService.UpdateModule(updatedDto);


      return response;
    }
  }
  */
}
