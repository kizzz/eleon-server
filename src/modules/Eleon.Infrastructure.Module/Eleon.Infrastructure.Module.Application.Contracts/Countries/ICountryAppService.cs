using System.Collections.Generic;
using System.Threading.Tasks;

namespace VPortal.Infrastructure.Module.Countries
{
  public interface ICountryAppService
  {
    Task<CountryDto> GetAsync(Guid id);
    Task<List<CountryDto>> GetCountryList();
  }
}
