using Duende.IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityModel;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants
{
  [Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
  [ExposeServices(typeof(IIdentityModelAuthenticationService), typeof(IRemoteServiceTokenManager))]
  public class IdentityModelWithExtensionGrantsAuthenticationService : IdentityModelAuthenticationService, IRemoteServiceTokenManager
  {
    private readonly IEnumerable<IExtensionGrantTokenRequestProvider> extensionGrantTokenRequestProviders;
    private readonly IEnumerable<IAccessTokenChangeObserver> accessTokenChangeObservers;

    public IdentityModelWithExtensionGrantsAuthenticationService(
        IOptions<AbpIdentityClientOptions> options,
        ICancellationTokenProvider cancellationTokenProvider,
        IHttpClientFactory httpClientFactory,
        ICurrentTenant currentTenant,
        IOptions<IdentityModelHttpRequestMessageOptions> identityModelHttpRequestMessageOptions,
        IDistributedCache<IdentityModelTokenCacheItem> tokenCache,
        IDistributedCache<IdentityModelDiscoveryDocumentCacheItem> discoveryDocumentCache,
        IEnumerable<IExtensionGrantTokenRequestProvider> extensionGrantTokenRequestProviders,
        IEnumerable<IAccessTokenChangeObserver> accessTokenChangeObservers,
        IAbpHostEnvironment abpHostEnv)
        : base(options, cancellationTokenProvider, httpClientFactory, currentTenant, identityModelHttpRequestMessageOptions, tokenCache, discoveryDocumentCache, abpHostEnv)
    {
      this.extensionGrantTokenRequestProviders = extensionGrantTokenRequestProviders;
      this.accessTokenChangeObservers = accessTokenChangeObservers;
    }

    protected override async Task<TokenResponse> GetTokenResponse(IdentityClientConfiguration configuration)
    {
      var requestProvider = GetTokenRequestProvider(configuration);
      TokenResponse response;
      if (requestProvider is not null)
      {
        var request = await GetTokenRequest(requestProvider, configuration);
        response = await SendTokenRequest(request);
      }
      else
      {
        response = await base.GetTokenResponse(configuration);
      }

      var tokenCacheItem = new IdentityModelTokenCacheItem(response.AccessToken);
      await TokenCache.SetAsync(
          IdentityModelTokenCacheItem.CalculateCacheKey(configuration),
          tokenCacheItem,
          new DistributedCacheEntryOptions
          {
            AbsoluteExpirationRelativeToNow = AbpHostEnvironment.IsDevelopment()
                  ? TimeSpan.FromSeconds(5)
                  : TimeSpan.FromSeconds(configuration.CacheAbsoluteExpiration),
          });

      foreach (var observer in accessTokenChangeObservers)
      {
        await observer.OnAccessTokenChangeAsync(response.AccessToken);
      }

      return response;
    }

    private async Task<TokenResponse> SendTokenRequest(TokenRequest tokenRequest)
    {
      using (var httpClient = HttpClientFactory.CreateClient(HttpClientName))
      {
        AddHeaders(httpClient);
        var response = await httpClient.RequestTokenAsync(tokenRequest, CancellationTokenProvider.Token);
        return response;
      }
    }

    private async Task<TokenRequest> GetTokenRequest(IExtensionGrantTokenRequestProvider requestProvider, IdentityClientConfiguration configuration)
    {
      var discoveryResponse = await GetDiscoveryResponse(configuration);
      var request = await requestProvider.GetTokenRequest(configuration);
      request.Address = discoveryResponse.TokenEndpoint;

      IdentityModelHttpRequestMessageOptions.ConfigureHttpRequestMessage?.Invoke(request);

      await AddParametersToRequestAsync(configuration, request);

      return request;
    }

    private IExtensionGrantTokenRequestProvider GetTokenRequestProvider(IdentityClientConfiguration configuration)
        => extensionGrantTokenRequestProviders.FirstOrDefault(x => x.GrantType == configuration.GrantType);

    public async Task ForgetAllTokens()
    {
      // TODO: Clear cache
    }
  }
}
