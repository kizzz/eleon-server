using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Accounting.Module.Constants;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.EntityFrameworkCore;
using VPortal.Accounts.Module.Extensions;

namespace VPortal.Accounting.Module.Repositories
{
  public class AccountRepository : EfCoreRepository<AccountingDbContext, AccountEntity, Guid>, IAccountRepository
  {
    private readonly IVportalLogger<AccountRepository> logger;

    public AccountRepository(IDbContextProvider<AccountingDbContext> dbContextProvider, IVportalLogger<AccountRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public override async Task<IQueryable<AccountEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeDetails();
    }

    public async Task<AccountEntity> GetAccountByAccountTenantId(Guid accountTenantId)
    {
      AccountEntity result = null;
      try
      {
        var dbContext = await GetDbContextAsync();
        result = dbContext.Accounts
                        .FirstOrDefault(x => x.TenantId.HasValue && x.TenantId.Value == accountTenantId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<KeyValuePair<long, List<AccountEntity>>> GetByFilter(
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
    IList<Guid> organizationUnitFilter = null)
    {
      KeyValuePair<long, List<AccountEntity>> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        string pattern = searchQuery == null ? null : $"%{searchQuery}%";

        var filtered = dbContext.Accounts
            .WhereIf(creationDateFilterStart != null, x => x.CreationTime >= creationDateFilterStart)
            .WhereIf(creationDateFilterEnd != null, x => x.CreationTime <= creationDateFilterEnd)

            .WhereIf(creationDateFilterStart != null, x => x.CreationTime >= creationDateFilterStart)
            .WhereIf(creationDateFilterEnd != null, x => x.CreationTime <= creationDateFilterEnd)
            .WhereIf(organizationUnitFilter != null,
                    x => organizationUnitFilter.Contains((Guid)x.OrganizationUnitId))
            .WhereIf(accountStatusFilter != null,
                    x => accountStatusFilter.Contains(x.AccountStatus))
            .WhereIf(searchQuery != null,
                x => EF.Functions.Like(x.Id.ToString(), pattern));

        var paginated = filtered
           .OrderBy(sorting)
           .ThenByDescending(x => x.CreationTime)
           .Skip(skipCount)
           .Take(maxResultCount);

        result = new(await filtered.CountAsync(), await paginated.ToListAsync());
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
