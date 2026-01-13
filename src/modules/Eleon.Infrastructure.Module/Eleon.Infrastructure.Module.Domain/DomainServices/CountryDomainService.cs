using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Infrastructure.Module.Entities;
using VPortal.Infrastructure.Module.Repositories;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{

  public class CountryDomainService : DomainService, ISingletonDependency
  {

    private readonly ICountryRepository countryRepository;
    private readonly IVportalLogger<CountryDomainService> logger;

    public CountryDomainService(
        ICountryRepository countryRepository,
        IVportalLogger<CountryDomainService> logger)
    {
      this.countryRepository = countryRepository;
      this.logger = logger;
    }

    public async Task<CountryEntity> GetAsync(Guid id)
    {
      CountryEntity result = null;
      try
      {
        result = await countryRepository.GetAsync(id);
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

    public async Task<List<CountryEntity>> GetCountryList()
    {
      List<CountryEntity> result = new List<CountryEntity>();
      try
      {
        result = await countryRepository.GetListAsync();
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
