using Common.Module.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using TenantSettings.Module.Cache;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Authorization.Module.ClientIsolation
{

  public class ClientIsolationValidator : ITransientDependency
  {
    private const string ClientCertHeader = "X-ARR-ClientCert";
    private const string ForwardedForHeader = "X-Forwarded-For";

    private readonly ILogger<ClientIsolationValidator> logger;
    private readonly TenantSettingsCacheService tenantSettingsCache;
    private readonly ICurrentTenant currentTenant;

    public ClientIsolationValidator(
        ILogger<ClientIsolationValidator> logger,
        TenantSettingsCacheService tenantSettingsCache,
        ICurrentTenant currentTenant)
    {
      this.logger = logger;
      this.tenantSettingsCache = tenantSettingsCache;
      this.currentTenant = currentTenant;
    }

    public async Task<ClientIsolationContextValidationResult> ValidateClientIsolation(HttpContext httpContext, Guid? userId)
    {
      ClientIsolationContextValidationResult result = null;
      try
      {
        var requestorIp = GetRequestorIpFromContext(httpContext);
        var certificate = GetCertificateFromContext(httpContext);
        result = await PassesClientIsolation(userId, requestorIp, certificate);
        result.Ip = requestorIp;
        result.ValidatedCertificateName = certificate?.Subject;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Error occurred while validating client isolation");
      }

      return result;
    }

    private async Task<ClientIsolationContextValidationResult> PassesClientIsolation(Guid? userId, string requestorIp, X509Certificate2 certificate)
    {
      ClientIsolationContextValidationResult ipIsolationResult = new(await PassesIpIsolation(requestorIp));
      if (!ipIsolationResult.Valid)
      {
        return ipIsolationResult;
      }

      ClientIsolationContextValidationResult certIsolationResult = new(await PassesCertificateIsolation(userId, certificate));
      return certIsolationResult;
    }

    private async Task<ClientIsolationValidationResult> PassesIpIsolation(string requestorIp)
    {
      var settings = await tenantSettingsCache.GetTenantSettings(currentTenant.Id);
      var tenantIps = settings.TenantWhitelistedIps;
      if (tenantIps == null || !tenantIps.Any())
      {
        return ClientIsolationValidationResult.NothingToValidate;
      }

      if (requestorIp.IsNullOrEmpty())
      {
        return ClientIsolationValidationResult.InvalidIp;
      }

      return tenantIps.Contains(requestorIp)
          ? ClientIsolationValidationResult.ValidIp
          : ClientIsolationValidationResult.InvalidIp;
    }

    private async Task<ClientIsolationValidationResult> PassesCertificateIsolation(Guid? userId, X509Certificate2 incomingCertificate)
    {
      var tenantSettings = await tenantSettingsCache.GetTenantSettings(currentTenant.Id);

      bool tenantIsolationEnabled = !tenantSettings.TenantCertificate.IsNullOrEmpty();
      if (tenantIsolationEnabled)
      {
        return ValidateCertificate(incomingCertificate, tenantSettings.TenantCertificate, ClientIsolationValidationResult.ValidTenantCert, ClientIsolationValidationResult.InvalidTenantCert);
      }

      if (!userId.HasValue)
      {
        return ClientIsolationValidationResult.NothingToValidate;
      }

      var etalonUserCertHash = tenantSettings.CertificatesByUsersLookup.GetValueOrDefault(userId.Value, null);
      bool userIsolationEnabled = !etalonUserCertHash.IsNullOrEmpty();
      if (userIsolationEnabled)
      {
        var userCertValidation = ValidateCertificate(
            incomingCertificate,
            etalonUserCertHash!,
            ClientIsolationValidationResult.ValidUserCert,
            ClientIsolationValidationResult.InvalidUserCert);
        if (ClientIsolationContextValidationResult.IsValid(userCertValidation))
        {
          return userCertValidation;
        }

        bool isAdminCertificateProvided = false;
        if (incomingCertificate != null)
        {
          string incomingCertHash = CertificateHelper.GetCertificateHash(incomingCertificate);
          var hostSettings = await tenantSettingsCache.GetTenantSettings(null);
          var hostUserId = hostSettings.UsersByCertificatesLookup.GetValueOrDefault(incomingCertHash, Guid.Empty);
          isAdminCertificateProvided = hostSettings.AdminUserIds.Contains(hostUserId);
        }

        if (isAdminCertificateProvided)
        {
          return ClientIsolationValidationResult.ValidUserCert;
        }
        else
        {
          return userCertValidation;
        }
      }

      return ClientIsolationValidationResult.CertIsolationDisabled;
    }

    private static string GetRequestorIpFromContext(HttpContext httpContext)
    {
      string forwardedForHeader = GetForwardedForHeader(httpContext);
      if (!string.IsNullOrWhiteSpace(forwardedForHeader))
      {
        var remoteIp = httpContext.Request.HttpContext.Connection.RemoteIpAddress;
        //if (IpAddressHelper.IsPrivateIPAddress(remoteIp.ToString()))
        //{
        //    throw new Exception("Forwarded from invalid IP.");
        //}

        var forwardedForHeaderIp = forwardedForHeader;

        if (forwardedForHeader.IndexOf(':') != -1)
        {
          forwardedForHeaderIp = forwardedForHeader.Remove(forwardedForHeader.IndexOf(':'));
        }

        var forwardedIp = IPAddress.Parse(forwardedForHeaderIp);
        return forwardedIp.ToString();
      }
      else
      {
        return httpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
      }
    }

    private static string GetForwardedForHeader(HttpContext httpContext)
    {
      if (httpContext.Request.Headers.TryGetValue(ForwardedForHeader, out var values))
      {
        return values.FirstOrDefault();
      }

      return null;
    }

    private static ClientIsolationValidationResult ValidateCertificate(
        X509Certificate2 certificate,
        string certificateHash,
        ClientIsolationValidationResult validResult,
        ClientIsolationValidationResult invalidResult)
    {
      if (certificate == null)
      {
        return ClientIsolationValidationResult.MissingClientCert;
      }

      return CertificateHelper.ValidateCertificateHash(certificate, certificateHash) ? validResult : invalidResult;
    }

    private static X509Certificate2 GetCertificateFromContext(HttpContext context)
    {
      if (context.Connection.ClientCertificate != null)
      {
        return context.Connection.ClientCertificate;
      }

      if (context.Request.Headers.TryGetValue(ClientCertHeader, out var certHeader))
      {
        var certString = certHeader.First();
        return X509CertificateLoader.LoadCertificate(certString.GetBytes());
      }

      return null;
    }
  }
}
