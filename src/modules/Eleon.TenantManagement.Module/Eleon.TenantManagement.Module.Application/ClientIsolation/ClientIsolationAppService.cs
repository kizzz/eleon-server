using Logging.Module;
using System;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.TenantIsolation;

namespace VPortal.TenantManagement.Module.ClientIsolation
{
  public class ClientIsolationAppService : TenantManagementAppService, IClientIsolationAppService
  {
    private readonly IVportalLogger<ClientIsolationAppService> logger;
    private readonly ClientIsolationDomainService clientIsolationDomainService;

    public ClientIsolationAppService(
        IVportalLogger<ClientIsolationAppService> logger,
        ClientIsolationDomainService clientIsolationDomainService)
    {
      this.logger = logger;
      this.clientIsolationDomainService = clientIsolationDomainService;
    }

    public async Task<UserIsolationSettingsDto> GetUserIsolationSettings(Guid userId)
    {
      UserIsolationSettingsDto result = null;
      try
      {
        var entity = await clientIsolationDomainService.GetUserIsolationSettings(userId);
        result = ObjectMapper.Map<UserIsolationSettingsEntity, UserIsolationSettingsDto>(entity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetTenantIpIsolationSettings(SetIpIsolationRequestDto request)
    {
      bool result = false;
      try
      {
        var whitelistedIps = ObjectMapper.Map<List<TenantWhitelistedIpDto>, List<TenantWhitelistedIpEntity>>(request.WhitelistedIps);
        await clientIsolationDomainService.SetTenantIpIsolationSettingsWithReplication(request.TenantId, request.IpIsolationEnabled, whitelistedIps);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetTenantIsolation(SetTenantIsolationRequestDto request)
    {
      bool result = false;
      try
      {
        var certificateBytes = MapCertificate(request.CertificatePemBase64);
        await clientIsolationDomainService.SetTenantIsolationWithReplication(request.TenantId, request.Enabled, certificateBytes, request.Password);

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> SetUserIsolation(SetUserIsolationRequestDto request)
    {
      bool result = false;
      try
      {
        var certificateBytes = MapCertificate(request.ClientCertificateBase64);
        await clientIsolationDomainService.SetUserIsolationWithReplication(request.UserId, request.Enabled, certificateBytes, request.Password);

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> ValidateClientIsolation(ValidateClientIsolationDto validateClientIsolationDto)
    {
      bool result = false;
      try
      {
        //await clientIsolationDomainService.Validate();

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    private byte[] MapCertificate(string certificateBase64)
        => certificateBase64.IsNullOrWhiteSpace()
                ? null
                : Convert.FromBase64String(certificateBase64);
  }
}
