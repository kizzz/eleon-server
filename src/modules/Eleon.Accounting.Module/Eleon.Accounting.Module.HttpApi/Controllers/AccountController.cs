using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Accounting.Module.Accounts;
// ❌ DEPRECATED: DocumentVersionEntity import removed
// using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.Controllers
{
  [Area(AccountingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AccountingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/account/accounts")]
  public class AccountController : ModuleController, IAccountAppService
  {
    private readonly IAccountAppService appService;
    private readonly IVportalLogger<AccountController> logger;

    public AccountController(
        IVportalLogger<AccountController> logger,
        IAccountAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("CancelAccount")]
    public async Task<string> CancelAccount(Guid id)
    {

      var response = await appService.CancelAccount(id);


      return response;
    }

    [HttpPost("CreateAccount")]
    public async Task<AccountDto> CreateAccount(CreateAccountDto dto)
    {

      var response = await appService.CreateAccount(dto);


      return response;
    }

    [HttpGet("GetAccountDetailsById")]
    public async Task<AccountDto> GetAccountDetailsById(Guid id)
    {

      var response = await appService.GetAccountDetailsById(id);


      return response;
    }

    [HttpPost("UpdateAccount")]
    // ❌ DEPRECATED: Changed return type from DocumentVersionEntity to AccountDto
    public async Task<AccountDto> UpdateAccount(AccountDto updatedAccount)
    {

      var response = await appService.UpdateAccount(updatedAccount);


      return response;
    }

    [HttpPost("ResendAccountInfo")]
    public async Task<string> ResendAccountInfo(Guid id)
    {

      var response = await appService.ResendAccountInfo(id);


      return response;
    }

    [HttpPost("ActivateAccount")]
    public async Task<string> ActivateAccount(Guid id)
    {

      var response = await appService.ActivateAccount(id);


      return response;
    }

    [HttpPost("ResetAccount")]
    public async Task<string> ResetAccount(Guid id)
    {

      var response = await appService.ResetAccount(id);


      return response;
    }

    [HttpPost("SuspendAccount")]
    public async Task<string> SuspendAccount(Guid id)
    {

      var response = await appService.SuspendAccount(id);


      return response;
    }

    [HttpPost("GetByFilter")]
    public async Task<PagedResultDto<AccountHeaderDto>> GetByFilter(AccountListRequestDto input)
    {

      var response = await appService.GetByFilter(input);


      return response;
    }
  }
}
