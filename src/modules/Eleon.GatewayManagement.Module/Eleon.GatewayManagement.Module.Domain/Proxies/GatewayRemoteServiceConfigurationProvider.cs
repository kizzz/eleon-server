using Common.Module.Constants;
using Microsoft.Extensions.Options;
using GatewayManagement.Module.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;

namespace GatewayManagement.Module.Proxies
{
  public class GatewayRemoteServiceConfigurationProvider : IRemoteServiceConfigurationProvider, IScopedDependency
  {
    private readonly IDistributedCache<RemoteServiceConfigurationDictionary, string> configCache;
    private readonly IOptions<AbpRemoteServiceOptions> options;
    private readonly CurrentGateway currentGateway;
    private readonly GatewayManagementDomainService gatewayManagementDomainService;

    public GatewayRemoteServiceConfigurationProvider(
        IDistributedCache<RemoteServiceConfigurationDictionary, string> configCache,
        IOptions<AbpRemoteServiceOptions> options,
        CurrentGateway currentGateway,
        GatewayManagementDomainService gatewayManagementDomainService)
    {
      this.configCache = configCache;
      this.options = options;
      this.currentGateway = currentGateway;
      this.gatewayManagementDomainService = gatewayManagementDomainService;
      EnsureDefaultOptionIsSet();
    }

    public async Task<RemoteServiceConfiguration> GetConfigurationOrDefaultAsync(string name)
    {
      var cfg = await GetCurrentGatewayRemoteServicesConfiguration();
      return cfg.GetConfigurationOrDefault(name);
    }

    public async Task<RemoteServiceConfiguration> GetConfigurationOrDefaultOrNullAsync(string name)
    {
      var cfg = await GetCurrentGatewayRemoteServicesConfiguration();
      return cfg.GetConfigurationOrDefaultOrNull(name);
    }

    private void EnsureDefaultOptionIsSet()
    {
      if (!options.Value.RemoteServices.ContainsKey(RemoteServiceConfigurationDictionary.DefaultName))
      {
        options.Value.RemoteServices.Default = new RemoteServiceConfiguration(string.Empty);
      }
    }

    private async Task<RemoteServiceConfigurationDictionary> GetCurrentGatewayRemoteServicesConfiguration()
    {
      if (currentGateway.Options == null)
      {
        throw new Exception("No gateway was set as a current for this scope.");
      }

      return await GetGatewayRemoteServicesConfiguration(currentGateway.Options.GatewayId, options.Value.RemoteServices);
    }

    private async Task<RemoteServiceConfigurationDictionary> GetGatewayRemoteServicesConfiguration(Guid gatewayId, RemoteServiceConfigurationDictionary preset)
        => await configCache.GetOrAddAsync(GetGatewayKey(gatewayId), () => CreateConfiguration(gatewayId, preset));

    private async Task<RemoteServiceConfigurationDictionary> CreateConfiguration(Guid gatewayId, RemoteServiceConfigurationDictionary preset)
    {
      var gateway = await gatewayManagementDomainService.GetGateway(gatewayId);
      if (gateway == null)
      {
        throw new ArgumentException("The gateway with the given id could not be found.", nameof(gatewayId));
      }

      //if (gateway.Protocol != Common.Module.Constants.GatewayProtocol.HTTPS)
      //{
      //    throw new Exception("Can not create remote service configuration for a non-HTTPS gateway.");
      //}

      return OverrideConfigurationPreset(gateway, preset);
    }

    private RemoteServiceConfigurationDictionary OverrideConfigurationPreset(GatewayEntity gateway, RemoteServiceConfigurationDictionary preset)
    {
      var overridenDictionary = new RemoteServiceConfigurationDictionary();
      foreach (var (serviceName, origCfg) in preset)
      {
        var overridenCfg = new RemoteServiceConfiguration();
        foreach (var (key, value) in origCfg)
        {
          overridenCfg[key] = OverrideValue(gateway, key, value);
        }

        overridenCfg.SetIdentityClient(gateway.Id.ToString());

        overridenDictionary[serviceName] = overridenCfg;
      }

      return overridenDictionary;
    }

    private string OverrideValue(GatewayEntity gateway, string key, string value)
    {
      if (key == nameof(RemoteServiceConfiguration.BaseUrl))
      {
        return GetHttpGatewayBaseUrl(gateway, value);
      }

      return value;
    }

    private string GetHttpGatewayBaseUrl(GatewayEntity gateway, string originalUrl)
    {
      if (gateway.Protocol == GatewayProtocol.HTTPS)
      {
        return $"https://{gateway.IpAddress}:{gateway.Port}";
      }
      else if (gateway.Protocol == GatewayProtocol.WSS)
      {
        return $"https://localhost";
      }

      return string.Empty;
    }

    private string GetGatewayKey(Guid gatewayId) => $"RemoteGatewayConfiguration:{gatewayId}";
  }
}
