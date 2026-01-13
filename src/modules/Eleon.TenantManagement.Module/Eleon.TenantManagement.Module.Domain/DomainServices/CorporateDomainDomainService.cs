using Common.Module.Constants;
using Logging.Module;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using Volo.Abp.Domain.Services;
using Volo.Abp.Validation;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Localization;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public partial class CorporateDomainDomainService : DomainService
  {
    private readonly IVportalLogger<CorporateDomainDomainService> logger;
    private readonly TenantHostnameDomainService tenantHostnameDomainService;
    private readonly IStringLocalizer<TenantManagementResource> localizer;

    public CorporateDomainDomainService(
        IVportalLogger<CorporateDomainDomainService> logger,
        TenantHostnameDomainService tenantHostnameDomainService,
        IStringLocalizer<TenantManagementResource> localizer)
    {
      this.logger = logger;
      this.tenantHostnameDomainService = tenantHostnameDomainService;
      this.localizer = localizer;
    }

    public async Task<List<TenantHostnameEntity>> GetTenantHostnamesAsync(Guid? tenantId)
    {
      try
      {
        return await tenantHostnameDomainService.GetTenantHostnames(tenantId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return new List<TenantHostnameEntity>();
    }

    public async Task<List<TenantHostnameEntity>> GetApplicationHostnamesAsync(Guid? tenantId, Guid? applicationId)
    {
      try
      {
        return await tenantHostnameDomainService.GetApplicationHostnames(tenantId, applicationId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return new List<TenantHostnameEntity>();
    }

    public async Task AddCorporateDomain(
        Guid? tenantId,
        string domainName,
        byte[] certificatePem,
        string password,
        bool acceptsClientCertificate,
        bool isSsl,
        bool isDefault,
        int port,
        Guid? appId)
    {
      try
      {
        //EnsurePemCertificateValid(domainName, certificatePem, password);
        EnsureDomainNameValid(domainName);

        var hostname = await tenantHostnameDomainService.AddNonInternalTenantHostname(
            tenantId,
            domainName,
            null,
            acceptsClientCertificate,
            isSsl,
            VportalApplicationType.Undefined,
            isDefault,
            port,
            appId);

        //iisDomainService.CreateIISBindings(GetBindingInfos(hostname));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task UpdateCorporateDomain(
        Guid? tenantId,
        Guid hostnameId,
        string domainName,
        byte[] certificatePem,
        string password,
        bool acceptsClientCertificate,
        bool isSsl,
        bool isDefault,
        int port,
        Guid? appId
        )
    {
      try
      {
        await RemoveCorporateDomain(tenantId, hostnameId);
        await AddCorporateDomain(tenantId, domainName, certificatePem, password, acceptsClientCertificate, isSsl, isDefault, port, appId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task RemoveCorporateDomain(Guid? tenantId, Guid hostnameId)
    {
      try
      {
        var removed = await tenantHostnameDomainService.RemoveNonInternalTenantHostname(tenantId, hostnameId);
        //iisDomainService.RemoveIISBindings(GetBindingInfos(removed));
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    //private List<IisBindingInfo> GetBindingInfos(TenantHostnameEntity hostname) =>
    //    hostname.GetHostname(withPort: false)
    //        .ToSingleItemList()
    //        .Select(x => new IisBindingInfo(VportalApplicationType.Undefined, x, hostname.Port, hostname.AcceptsClientCertificate))
    //        .ToList();

    //private void EnsurePemCertificateValid(string hostname, byte[] certificatePem, string password)
    //{
    //    bool certificateValid = iisManager.ValidatePemCeritificate(hostname, certificatePem, password);
    //    if (!certificateValid)
    //    {
    //        throw new UserFriendlyException(localizer["CreateIisBinding:Error:CertificateInvalid"]);
    //    }
    //}

    private void EnsureDomainNameValid(string domainName)
    {
      bool domainNameValid = DomainNameRegex().IsMatch(domainName);
      if (!domainNameValid)
      {
        throw new AbpValidationException("Trying to add invalid corporate domain.");
      }
    }

    [GeneratedRegex("^(([\\da-zA-Z])([_\\w-]{0,62})\\.){0,127}(([\\da-zA-Z])[_\\w-]{0,61})?([\\da-zA-Z]\\.((xn\\-\\-[a-zA-Z\\d]+)|([a-zA-Z\\d]{2,})))$")]
    private static partial Regex DomainNameRegex();
  }
}
