using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using VPortal.Infrastructure.Module;
using VPortal.Infrastructure.Module.Addresses;

namespace VPortal.Addresses.Feature.Module.Controllers
{
  [Area(InfrastructureRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = InfrastructureRemoteServiceConsts.RemoteServiceName)]
  [Route("api/address/addresses")]
  public class AddressController : InfrastructureController, IAddressAppService
  {
    private readonly IAddressAppService appService;
    private readonly IVportalLogger<AddressController> _logger;

    public AddressController(
        IVportalLogger<AddressController> logger,
        IAddressAppService appService)
    {
      _logger = logger;
      this.appService = appService;
    }

    [HttpGet("GetAddressById")]
    public async Task<AddressDto> GetAddressById(Guid id)
    {

      var response = await appService.GetAddressById(id);


      return response;
    }

    [HttpPost("CreateAddress")]
    public async Task<AddressDto> CreateAddress(AddressDto createdDto)
    {

      var response = await appService.CreateAddress(createdDto);


      return response;
    }
  }
}
