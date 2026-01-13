using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.SitesManagement.Module;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.ApplicationConnectionStrings
{
  public class ApplicationConnectionStringAppService : SitesManagementAppService, IApplicationConnectionStringAppService
  {
    private readonly IVportalLogger<ApplicationConnectionStringAppService> logger;
    private readonly TenantApplicationConnectionStringDomainService connectionStringDomainService;

    public ApplicationConnectionStringAppService(
        IVportalLogger<ApplicationConnectionStringAppService> logger,
        TenantApplicationConnectionStringDomainService connectionStringDomainService)
    {
      this.logger = logger;
      this.connectionStringDomainService = connectionStringDomainService;
    }

    public async Task<bool> AddConnectionString(CreateConnectionStringRequestDto request)
    {
      bool result = false;
      try
      {
        await connectionStringDomainService.AddConnectionString(
            request.TenantId,
            request.ApplicationName,
            request.Status,
            request.ConnectionString);

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<ConnectionStringDto?> GetAsync(Guid? tenantId, string applicationName)
    {
      ConnectionStringDto? result = null;
      try
      {
        var conString = await connectionStringDomainService.GetAsync(tenantId, applicationName);
        result = conString == null ? null : ObjectMapper.Map<ApplicationTenantConnectionStringEntity, ConnectionStringDto>(conString);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetConnectionStringAsync(SetConnectionStringRequestDto request)
    {
      try
      {
        await connectionStringDomainService.SetConnectionStringAsync(request.TenantId, request.ApplicationName, request.ConnectionString);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<ConnectionStringDto>> GetConnectionStrings(Guid tenantId)
    {
      List<ConnectionStringDto> result = null;
      try
      {
        var conStrings = await connectionStringDomainService.GetConnectionStrings(tenantId);

        result = ObjectMapper.Map<List<ApplicationTenantConnectionStringEntity>, List<ConnectionStringDto>>(conStrings);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> RemoveConnectionString(RemoveConnectionStringRequestDto request)
    {
      bool result = false;
      try
      {
        await connectionStringDomainService.RemoveConnectionString(request.TenantId, request.ApplicationName);
        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> UpdateConnectionString(UpdateConnectionStringRequestDto request)
    {
      bool result = false;
      try
      {
        await connectionStringDomainService.UpdateConnectionString(
            request.TenantId,
            request.ApplicationName,
            request.ConnectionString,
            request.Status);

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


