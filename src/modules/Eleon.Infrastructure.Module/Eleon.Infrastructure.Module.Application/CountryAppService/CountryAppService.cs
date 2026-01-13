using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.Infrastructure.Module.Domain.DomainServices;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Infrastructure.Module.Countries
{
  public class CountryAppService : InfrastructureAppService, ICountryAppService
  {
    private readonly IVportalLogger<CountryAppService> logger;
    private readonly CountryDomainService domainService;

    public CountryAppService(
        IVportalLogger<CountryAppService> logger,
        CountryDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<CountryDto> GetAsync(Guid id)
    {
      CountryDto countryList = null;
      try
      {
        CountryEntity entity = await domainService.GetAsync(id);
        countryList = ObjectMapper.Map<CountryEntity, CountryDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return countryList;
    }


    public async Task<List<CountryDto>> GetCountryList()
    {
      List<CountryDto> countryList = new List<CountryDto>();
      try
      {
        List<CountryEntity> list = await domainService.GetCountryList();
        countryList = ObjectMapper.Map<List<CountryEntity>, List<CountryDto>>(list);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return countryList;
    }
  }
}
