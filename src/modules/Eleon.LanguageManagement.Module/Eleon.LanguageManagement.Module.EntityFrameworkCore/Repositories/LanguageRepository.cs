using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.EntityFrameworkCore;

namespace VPortal.LanguageManagement.Module.Repositories
{
  public class LanguageRepository :
      EfCoreRepository<LanguageManagementDbContext, LanguageEntity, Guid>,
      ILanguageRepository
  {
    public LanguageRepository(IDbContextProvider<LanguageManagementDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
  }
}
