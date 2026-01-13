using System;
using Volo.Abp.Domain.Repositories;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Infrastructure.Module.Repositories
{
  public interface ICountryRepository : IBasicRepository<CountryEntity, Guid>
  {
  }
}
