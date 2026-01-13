using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module.Repositories
{
  public interface IPackageTemplateRepository : IBasicRepository<PackageTemplateEntity, Guid>
  {
    Task<KeyValuePair<long, List<PackageTemplateEntity>>> GetListAsync(
               string sorting = null,
               int maxResultCount = int.MaxValue,
               int skipCount = 0,
               string searchQuery = null,
               DateTime? dateFilterStart = null,
               DateTime? dateFilterEnd = null,
               IList<BillingPeriodType> billingPeriodTypeFilter = null);
  }
}
