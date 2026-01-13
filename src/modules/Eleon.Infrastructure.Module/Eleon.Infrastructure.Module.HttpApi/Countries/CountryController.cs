using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Infrastructure.Module;
using VPortal.Infrastructure.Module.Countries;

namespace VPortal.Countries.Feature.Module.Controllers
{
  [Area(InfrastructureRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = InfrastructureRemoteServiceConsts.RemoteServiceName)]
  [Route("api/country/countries")]
  public class CountryController : InfrastructureController, ICountryAppService
  {
    private readonly ICountryAppService appService;
    private readonly IVportalLogger<CountryController> _logger;

    public CountryController(
        IVportalLogger<CountryController> logger,
        ICountryAppService appService)
    {
      _logger = logger;
      this.appService = appService;
    }

    [HttpGet("Get/{id}")]
    public async Task<CountryDto> GetAsync(Guid id)
    {

      var response = await appService.GetAsync(id);


      return response;
    }

    [HttpGet("GetCountryById")]
    public async Task<List<CountryDto>> GetCountryList()
    {

      var response = await appService.GetCountryList();


      return response;
    }
  }
}
