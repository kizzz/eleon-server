using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenantSettings.Module.Cache;
using Volo.Abp.Domain.Entities;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Permissions;

namespace VPortal.TenantManagement.Module.CorporateDomains;

[Authorize]
public class DomainSettingsAppService : TenantManagementAppService, IDomainSettingsAppService
{
  private readonly IVportalLogger<DomainSettingsAppService> logger;
  private readonly CorporateDomainDomainService corporateDomainDomainService;

  public DomainSettingsAppService(
      IVportalLogger<DomainSettingsAppService> logger,
      CorporateDomainDomainService corporateDomainDomainService)
  {
    this.logger = logger;
    this.corporateDomainDomainService = corporateDomainDomainService;
  }

  public async Task<List<TenantHostnameDto>> GetCurrentTenantHostnamesAsync()
  {

    try
    {
      var entities = await corporateDomainDomainService.GetTenantHostnamesAsync(CurrentTenant.Id);
      return ObjectMapper.Map<List<TenantHostnameEntity>, List<TenantHostnameDto>>(entities);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return [];
  }

  public async Task<List<TenantHostnameDto>> GetHostnamesByApplicationAsync(Guid? applicationId)
  {

    try
    {
      var entities = await corporateDomainDomainService.GetApplicationHostnamesAsync(CurrentTenant.Id, applicationId);
      return ObjectMapper.Map<List<TenantHostnameEntity>, List<TenantHostnameDto>>(entities);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return [];
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> AddCorporateDomainAsync(CreateCorporateDomainRequestDto request)
  {
    bool result = false;
    try
    {
      byte[] certificateBytes = new byte[] { };
      if (request.CertificatePemBase64.Length > 0)
      {
        certificateBytes = Convert.FromBase64String(request.CertificatePemBase64);
      }
      await corporateDomainDomainService.AddCorporateDomain(
          CurrentTenant.Id,
          request.DomainName,
          certificateBytes,
          request.Password,
          request.AcceptsClientCertificate,
          request.IsSsl,
          request.Default,
          request.Port,
          request.AppId);
      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> UpdateCorporateDomainAsync(UpdateCorporateDomainRequestDto request)
  {
    bool result = false;
    try
    {
      var certificateBytes = Convert.FromBase64String(request.CertificatePemBase64);
      await corporateDomainDomainService.UpdateCorporateDomain(
          CurrentTenant.Id,
          request.HostnameId,
          request.DomainName,
          certificateBytes,
          request.Password,
          request.AcceptsClientCertificate,
          request.IsSsl,
          request.Default,
          request.Port,
          request.AppId);

      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> UpdateDomainApplicationAsync(Guid domainId, Guid? appId)
  {

    try
    {
      var domains = await corporateDomainDomainService.GetTenantHostnamesAsync(CurrentTenant.Id);
      var domain = domains.FirstOrDefault(x => x.Id == domainId);
      if (domain == null)
      {
        throw new EntityNotFoundException();
      }
      await corporateDomainDomainService.UpdateCorporateDomain(CurrentTenant.Id, domainId, domain.Domain, [], string.Empty, domain.AcceptsClientCertificate, domain.IsSsl, false, domain.Port, appId);
      return true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
      return false;
    }
    finally
    {
    }
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> RemoveCorporateDomainAsync(Guid id)
  {
    bool result = false;
    try
    {

      await corporateDomainDomainService.RemoveCorporateDomain(CurrentTenant.Id, id);
      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<List<TenantHostnameDto>> GetHostnamesForTenantAsync(Guid? tenantId)
  {

    try
    {
      var entities = await corporateDomainDomainService.GetTenantHostnamesAsync(tenantId);
      return ObjectMapper.Map<List<TenantHostnameEntity>, List<TenantHostnameDto>>(entities);
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }
    finally
    {
    }

    return [];
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> AddCorporateDomainForTenantAsync(Guid? tenantId, CreateCorporateDomainRequestDto request)
  {
    bool result = false;
    try
    {
      byte[] certificateBytes = new byte[] { };
      if (request.CertificatePemBase64.Length > 0)
      {
        certificateBytes = Convert.FromBase64String(request.CertificatePemBase64);
      }
      await corporateDomainDomainService.AddCorporateDomain(
          tenantId,
          request.DomainName,
          certificateBytes,
          request.Password,
          request.AcceptsClientCertificate,
          request.IsSsl,
          request.Default,
          request.Port,
          request.AppId);
      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> UpdateCorporateDomainForTenantAsync(Guid? tenantId, UpdateCorporateDomainRequestDto request)
  {
    bool result = false;
    try
    {
      var certificateBytes = Convert.FromBase64String(request.CertificatePemBase64);
      await corporateDomainDomainService.UpdateCorporateDomain(
          tenantId,
          request.HostnameId,
          request.DomainName,
          certificateBytes,
          request.Password,
          request.AcceptsClientCertificate,
          request.IsSsl,
          request.Default,
          request.Port,
          request.AppId);

      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }

  [Authorize(TenantManagementPermissions.TenantSettingsManagement)]
  public async Task<bool> RemoveCorporateDomainForTenantAsync(Guid? tenantId, Guid domainId)
  {
    bool result = false;
    try
    {
      await corporateDomainDomainService.RemoveCorporateDomain(tenantId, domainId);
      result = true;
    }
    catch (Exception ex)
    {
      logger.Capture(ex);
    }

    return result;
  }
}
