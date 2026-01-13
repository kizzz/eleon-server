using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Accounting.Module.Constants;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module.Repositories
{
  public interface IAccountRepository : IBasicRepository<AccountEntity, Guid>
  {
    Task<AccountEntity> GetAccountByAccountTenantId(Guid accountTenantId);
    Task<KeyValuePair<long, List<AccountEntity>>> GetByFilter(
       AccountListRequestType requestType,
       Guid userId,
       IList<string> documentIds = null,
       string sorting = null,
       int maxResultCount = int.MaxValue,
       int skipCount = 0,
       string searchQuery = null,
       DateTime? creationDateFilterStart = null,
       DateTime? creationDateFilterEnd = null,
       string initiatorNameFilter = null,
       IList<AccountStatus> accountStatusFilter = null,
       IList<Guid> organizationUnitFilter = null);
  }
}
