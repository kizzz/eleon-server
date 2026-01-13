using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
// ❌ DEPRECATED: DocumentVersionEntity import removed
// using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.Accounts
{
  public interface IAccountAppService
  {
    Task<string> CancelAccount(Guid id);
    Task<AccountDto> GetAccountDetailsById(Guid id);
    Task<AccountDto> CreateAccount(CreateAccountDto dto);
    // ❌ DEPRECATED: Changed return type from DocumentVersionEntity to AccountDto
    Task<AccountDto> UpdateAccount(AccountDto updatedAccount);
    Task<string> ResendAccountInfo(Guid id);
    Task<string> ActivateAccount(Guid id);
    Task<string> ResetAccount(Guid id);
    Task<string> SuspendAccount(Guid id);
    Task<PagedResultDto<AccountHeaderDto>> GetByFilter(AccountListRequestDto input);
  }
}
