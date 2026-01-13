using Common.Module.Keys;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants.MachineKey;

namespace GatewayManagement.Module.Proxies
{
  [ExposeServices(typeof(IMachineSecretsProvider), IncludeSelf = true)]
  public class GatewayHostMachineSecretsProvider : IMachineSecretsProvider
  {
    private const string NoHttpContextErr =
        "Unable to resolve client key outside of the HTTP context. " +
        "Please, make sure you are using this service within an HTTP context and the " +
        "HttpContextAccessor is configured properly.";
    private const string NoGatewayErr =
        "Unable to retreive gateway keys as no current gateway is set.";
    private const string NoGatewayDetailsErr =
        "Unable to retreive gateway keys.";
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly GatewayManagementDomainService gatewayManagementDomainService;

    private Guid? _cachedTenantId;
    private Guid? CurrentTenantId => _cachedTenantId ??= httpContextAccessor.HttpContext.RequestServices.GetRequiredService<CurrentTenant>().Id;

    public GatewayHostMachineSecretsProvider(IHttpContextAccessor httpContextAccessor, GatewayManagementDomainService gatewayManagementDomainService)
    {
      this.httpContextAccessor = httpContextAccessor;
      this.gatewayManagementDomainService = gatewayManagementDomainService;
    }

    public async Task<string> GetClientCompoundKey()
    {
      var gatewayDetails = await GetCurrentGatewayDetails();
      var compoundKey = new ClientCompoundKey(gatewayDetails.clientKey, CurrentTenantId);
      return compoundKey.ToString();
    }

    public async Task<string> GetMachineKey()
    {
      var gatewayDetails = await GetCurrentGatewayDetails();
      return gatewayDetails.machineKey;
    }

    private async Task<(string machineKey, string clientKey)> GetCurrentGatewayDetails()
    {
      var httpContext = httpContextAccessor.HttpContext;
      if (httpContext == null)
      {
        throw new Exception(NoHttpContextErr);
      }

      var currentGateway = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<CurrentGateway>();
      if (currentGateway.Options?.GatewayId == null)
      {
        throw new Exception(NoGatewayErr);
      }

      var gatewayDetails = await gatewayManagementDomainService.GetGatewayPrivateDetails(CurrentTenantId, currentGateway.Options.GatewayId);
      if (gatewayDetails == null)
      {
        throw new Exception(NoGatewayDetailsErr);
      }

      return gatewayDetails.Value;
    }
  }
}
