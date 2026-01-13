using Logging.Module;
using System;
using System.Threading.Tasks;
using VPortal.Infrastructure.Module.Domain.DomainServices;
using VPortal.Infrastructure.Module.Entities;

namespace VPortal.Infrastructure.Module.Addresses
{
  public class AddressAppService : InfrastructureAppService, IAddressAppService
  {
    private readonly IVportalLogger<AddressAppService> logger;
    private readonly AddressDomainService domainService;

    public AddressAppService(
        IVportalLogger<AddressAppService> logger,
        AddressDomainService domainService)
    {
      this.logger = logger;
      this.domainService = domainService;
    }

    public async Task<AddressDto> CreateAddress(AddressDto createdDto)
    {
      AddressDto response = null;

      try
      {
        var createdEntity = ObjectMapper.Map<AddressDto, AddressEntity>(createdDto);
        var gotEntity = await domainService.CreateAddress(createdEntity);
        response = ObjectMapper.Map<AddressEntity, AddressDto>(gotEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<AddressDto> GetAddressById(Guid addressId)
    {
      AddressDto response = null;

      try
      {
        var gotEntity = await domainService.GetAddressById(addressId);
        response = ObjectMapper.Map<AddressEntity, AddressDto>(gotEntity);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }
  }
}
