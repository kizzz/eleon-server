using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.EntityFrameworkCore;
using VPortal.Accounts.Module.Extensions;

namespace VPortal.Accounting.Module.Repositories
{
  public class PackageTemplateRepository : EfCoreRepository<AccountingDbContext, PackageTemplateEntity, Guid>, IPackageTemplateRepository
  {
    private readonly IVportalLogger<PackageTemplateRepository> logger;

    public PackageTemplateRepository(IDbContextProvider<AccountingDbContext> dbContextProvider, IVportalLogger<PackageTemplateRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<KeyValuePair<long, List<PackageTemplateEntity>>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null,
        DateTime? dateFilterStart = null,
        DateTime? dateFilterEnd = null,
        IList<BillingPeriodType> billingPeriodTypeFilter = null)
    {
      KeyValuePair<long, List<PackageTemplateEntity>> result = new();
      try
      {
        var dbContext = await GetDbContextAsync();
        string pattern = searchQuery == null ? null : $"%{searchQuery}%";
        var filtered = dbContext.PackageTemplates
            .WhereIf(dateFilterStart != null, x => x.CreationTime >= dateFilterStart)
            .WhereIf(dateFilterEnd != null, x => x.CreationTime <= dateFilterEnd)
            .WhereIf(searchQuery != null,
                x => EF.Functions.Like(x.PackageName, pattern))
            .WhereIf(billingPeriodTypeFilter != null,
                    x => billingPeriodTypeFilter.Contains(x.BillingPeriodType));
        var paginated = filtered
           .OrderBy(sorting)
           .ThenByDescending(x => x.CreationTime)
           .Skip(skipCount)
           .Take(maxResultCount).IncludeDetails();
        result = new(await filtered.CountAsync(), await paginated.ToListAsync());
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return result;
    }

    public override async Task<IQueryable<PackageTemplateEntity>> WithDetailsAsync()
    {
      return (await GetQueryableAsync()).IncludeDetails();
    }
  }
}
