using Logging.Module;
using System;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;

namespace VPortal.TenantManagement.Module.ContentSecurityHosts
{
  public class TenantContentSecurityAppService : TenantManagementAppService, ITenantContentSecurityAppService
  {
    private readonly IVportalLogger<TenantContentSecurityAppService> logger;
    private readonly TenantContentSecurityDomainService corporateDomainDomainService;

    public TenantContentSecurityAppService(
        IVportalLogger<TenantContentSecurityAppService> logger,
        TenantContentSecurityDomainService corporateDomainDomainService)
    {
      this.logger = logger;
      this.corporateDomainDomainService = corporateDomainDomainService;
    }

    public async Task<bool> AddTenantContentSecurityHost(AddTenantContentSecurityHostDto input)
    {
      bool result = false;
      try
      {
        await corporateDomainDomainService.AddTenantContentSecurityHostWithReplication(input.TenantId, input.Hostname);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> RemoveTenantContentSecurityHost(RemoveTenantContentSecurityHostDto input)
    {
      bool result = false;
      try
      {
        await corporateDomainDomainService.RemoveTenantContentSecurityHostWithReplication(input.TenantId, input.ContentSecurityHostId);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> UpdateTenantContentSecurityHost(UpdateTenantContentSecurityHostDto input)
    {
      bool result = false;
      try
      {
        await corporateDomainDomainService.UpdateTenantContentSecurityHostWithReplication(input.TenantId, input.ContentSecurityHostId, input.NewHostname);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
