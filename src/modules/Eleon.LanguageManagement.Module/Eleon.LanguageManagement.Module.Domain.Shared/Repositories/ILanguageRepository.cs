using System;
using Volo.Abp.Domain.Repositories;
using VPortal.LanguageManagement.Module.Entities;

namespace VPortal.LanguageManagement.Module.Repositories
{
  public interface ILanguageRepository : IBasicRepository<LanguageEntity, Guid>
  {
  }
}
