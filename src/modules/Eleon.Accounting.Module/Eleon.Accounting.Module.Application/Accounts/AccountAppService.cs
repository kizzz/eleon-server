using Common.Module.Extensions;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using VPortal.Accounting.Module.AuditEntities;
using VPortal.Accounting.Module.DomainServices;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Permissions;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Accounting.Module.Accounts
{
  [Authorize(AccountingPermissions.General)]
  public class AccountAppService : ModuleAppService, IAccountAppService
  {
    private readonly IVportalLogger<AccountAppService> logger;
    private readonly AccountDomainService domainService;

    public AccountAppService(
        IVportalLogger<AccountAppService> logger,
        AccountDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<string> CancelAccount(Guid id)
    {
      string result = null;
      try
      {
        result = await domainService.CancelAccount(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(AccountingPermissions.General)]
    public async Task<AccountDto> GetAccountDetailsById(Guid id)
    {
      AccountDto result = null;
      try
      {
        var entity = await domainService.GetAccountDetailsById(id);
        result = ObjectMapper.Map<AccountEntity, AccountDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<AccountDto> CreateAccount(CreateAccountDto dto)
    {

      AccountDto response = null;
      try
      {
        var created = await domainService.CreateAccount(
            dto.DataSourceUid,
            dto.DataSourceName,
            dto.CompanyUid,
            dto.CompanyName,
            dto.OrganizationUnitId,
            dto.OrganizationUnitName,
            dto.OwnerId);

        // ❌ DEPRECATED: Changed return type from AccountAuditEntity to AccountEntity
        // response = ObjectMapper.Map<AccountAuditEntity, AccountDto>(created);
        response = ObjectMapper.Map<AccountEntity, AccountDto>(created);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<AccountDto> UpdateAccount(AccountDto updatedAccount)
    {
      AccountDto result = null;
      try
      {
        foreach (var member in updatedAccount.Members)
        {
          if (member.Id == Guid.Empty || member.Id.IsTempGuid())
          {
            member.Id = Guid.NewGuid();
          }
        }
        foreach (var accountPackage in updatedAccount.AccountPackages)
        {
          if (accountPackage.Id == Guid.Empty || accountPackage.Id.IsTempGuid())
          {
            accountPackage.Id = Guid.NewGuid();
          }
        }

        var mappedEntity = ObjectMapper.Map<AccountDto, AccountEntity>(updatedAccount);
        var updatedEntity = await domainService.UpdateAccount(mappedEntity);
        // ❌ DEPRECATED: Changed return type from DocumentVersionEntity to AccountEntity
        result = ObjectMapper.Map<AccountEntity, AccountDto>(updatedEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<string> ResendAccountInfo(Guid id)
    {
      string result = null;
      try
      {
        result = await domainService.ResendAccountInfo(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<string> ActivateAccount(Guid id)
    {
      string result = null;
      try
      {
        result = await domainService.ActivateAccount(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<string> ResetAccount(Guid id)
    {
      string result = null;
      try
      {
        result = await domainService.ResetAccount(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    [Authorize(AccountingPermissions.Create)]
    public async Task<string> SuspendAccount(Guid id)
    {
      string result = null;
      try
      {
        result = await domainService.SuspendAccount(id);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    [Authorize(AccountingPermissions.General)]
    public async Task<PagedResultDto<AccountHeaderDto>> GetByFilter(AccountListRequestDto input)
    {
      PagedResultDto<AccountHeaderDto> result = null;
      try
      {
        var entities = await domainService.GetByFilter(
            input.RequestType,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.SearchQuery,
            input.CreationDateFilterStart,
            input.CreationDateFilterEnd,
            input.InitiatorNameFilter,
            input.AccountStatusFilter,
            input.OrganizationUnitFilter);

        var dtos = ObjectMapper.Map<List<AccountEntity>, List<AccountHeaderDto>>(entities.Value);
        result = new PagedResultDto<AccountHeaderDto>(entities.Key, dtos);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
  }
}
