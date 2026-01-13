using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Accounting.Module.Accounts;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.Controllers
{
  // ❌ DEPRECATED: This entire controller is deprecated and commented out.
  // TODO: Remove or refactor this controller after migration is complete.
  /*
  [Area(AccountingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AccountingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/account/resellerAccounts")]
  public class ResellerAccountController : ModuleController, IResellerAccountAppService
  {
    private readonly IResellerAccountAppService appService;
    private readonly IVportalLogger<ResellerAccountController> logger;

    public ResellerAccountController(
        IVportalLogger<ResellerAccountController> logger,
        IResellerAccountAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("CreateAccountByReseller")]
    public async Task<AccountDto> CreateAccount(CreateAccountDto dto)
    {

      var response = await appService.CreateAccount(dto);


      return response;
    }

    [HttpGet("GetResellerAccountDetailsById")]
    public async Task<AccountDto> GetAccountDetailsById(Guid id, string requiredVersion)
    {

      var response = await appService.GetAccountDetailsById(id, requiredVersion);


      return response;
    }

    [HttpPost("UpdateAccountByReseller")]
    public async Task<DocumentVersionEntity> UpdateAccount(AccountDto updatedAccount, bool isDraft)
    {

      var response = await appService.UpdateAccount(updatedAccount, isDraft);


      return response;
    }

    [HttpPost("GetAccountInfoDetails")]
    public async Task<AccountDto> GetAccountInfoDetails()
    {

      var response = await appService.GetAccountInfoDetails();


      return response;
    }

    [HttpPost("UpdateAccountInfoDetails")]
    public async Task<DocumentVersionEntity> UpdateAccountInfoDetails(AccountDto updatedDto)
    {

      var response = await appService.UpdateAccountInfoDetails(updatedDto);


      return response;
    }

    [HttpPost("CancelAccountByReseller")]
    public async Task<string> CancelAccount(Guid id)
    {

      var response = await appService.CancelAccount(id);


      return response;
    }

    [HttpPost("ResendAccountInfoByReseller")]
    public async Task<string> ResendAccountInfo(Guid id)
    {

      var response = await appService.ResendAccountInfo(id);


      return response;
    }

    [HttpPost("ActivateAccountByReseller")]
    public async Task<string> ActivateAccount(Guid id)
    {

      var response = await appService.ActivateAccount(id);


      return response;
    }

    [HttpPost("ResetAccountByReseller")]
    public async Task<string> ResetAccount(Guid id)
    {

      var response = await appService.ResetAccount(id);


      return response;
    }

    [HttpPost("SuspendAccountByReseller")]
    public async Task<string> SuspendAccount(Guid id)
    {

      var response = await appService.SuspendAccount(id);


      return response;
    }

    [HttpPost("GetAccountListByReseller")]
    public async Task<PagedResultDto<AccountHeaderDto>> GetAccountListByReseller(AccountListRequestDto input)
    {

      var response = await appService.GetAccountListByReseller(input);


      return response;
    }
  }
  */
}
