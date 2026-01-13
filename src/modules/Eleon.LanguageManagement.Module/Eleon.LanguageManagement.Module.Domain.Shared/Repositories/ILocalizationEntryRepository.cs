using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.LanguageManagement.Module.Entities;

namespace VPortal.LanguageManagement.Module.Repositories
{
  public interface ILocalizationEntryRepository : IBasicRepository<LocalizationEntryEntity, Guid>
  {
    Task<LocalizationEntryEntity> FindByResourceAndKey(string culture, string resourceName, string key);
    Task<List<LocalizationEntryEntity>> GetByResource(string culture, string resourceName);
  }
}
