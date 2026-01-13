using Logging.Module;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.EntityFrameworkCore;

namespace VPortal.Accounting.Module.Repositories
{
  public class BillingInformationRepository : EfCoreRepository<AccountingDbContext, BillingInformationEntity, Guid>, IBillingInformationRepository
  {
    private readonly IVportalLogger<BillingInformationRepository> logger;

    public BillingInformationRepository(IDbContextProvider<AccountingDbContext> dbContextProvider, IVportalLogger<BillingInformationRepository> logger) : base(dbContextProvider)
    {
      this.logger = logger;
    }
  }
}
