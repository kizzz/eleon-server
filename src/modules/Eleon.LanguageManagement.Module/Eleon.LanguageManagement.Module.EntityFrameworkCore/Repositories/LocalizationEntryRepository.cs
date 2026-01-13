using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using VPortal.LanguageManagement.Module.Entities;
using VPortal.LanguageManagement.Module.EntityFrameworkCore;

namespace VPortal.LanguageManagement.Module.Repositories
{
  public class LocalizationEntryRepository :
      EfCoreRepository<LanguageManagementDbContext, LocalizationEntryEntity, Guid>,
      ILocalizationEntryRepository
  {
    private readonly IVportalLogger<LocalizationEntryRepository> logger;

    public LocalizationEntryRepository(
        IDbContextProvider<LanguageManagementDbContext> dbContextProvider,
        IVportalLogger<LocalizationEntryRepository> logger)
        : base(dbContextProvider)
    {
      this.logger = logger;
    }

    public async Task<LocalizationEntryEntity> FindByResourceAndKey(string culture, string resourceName, string key)
    {
      LocalizationEntryEntity result = null;
      try
      {
        var entries = await GetDbSetAsync();
        result = await entries.FirstOrDefaultAsync(x => x.CultureName == culture && x.Key == key && x.ResourceName == resourceName);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<LocalizationEntryEntity>> GetByResource(string culture, string resourceName)
    {
      List<LocalizationEntryEntity> result = null;
      try
      {
        var entries = await GetDbSetAsync();
        result = await entries
            .Where(x => x.CultureName == culture && x.ResourceName == resourceName)
            .ToListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
