using Logging.Module;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.EntityFrameworkCore;

namespace VPortal.Infrastructure.Module.Repositories
{
  public class CountryRepository : EfCoreRepository<InfrastructureDbContext, CountryEntity, Guid>, ICountryRepository
  {
    private readonly IVportalLogger<CountryRepository> logger;

    public CountryRepository(
        IDbContextProvider<InfrastructureDbContext> dbContextProvider,
        IVportalLogger<CountryRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }
  }
}
