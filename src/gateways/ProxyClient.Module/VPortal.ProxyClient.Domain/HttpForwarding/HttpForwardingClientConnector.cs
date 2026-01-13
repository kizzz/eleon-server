using ProxyManagement.Module.HttpForwarding;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.IdentityModel;
using VPortal.Identity.Module.AbpProxyExtensions.ExtensionGrants;
using Common.Module.Extensions;

namespace VPortal.ProxyClient.Domain.HttpForwarding
{
    [ExposeServices(typeof(IAccessTokenChangeObserver), IncludeSelf = true)]
    public class HttpForwardingClientConnector : ITransientDependency, IAccessTokenChangeObserver
    {
        private readonly HttpForwardingClient httpForwardingClient;
        private readonly IDistributedCache<IdentityModelTokenCacheItem> tokenCache;
        private readonly AbpIdentityClientOptions options;

        public HttpForwardingClientConnector(
            HttpForwardingClient httpForwardingClient,
            IDistributedCache<IdentityModelTokenCacheItem> tokenCache,
            IOptions<AbpIdentityClientOptions> options)
        {
            this.httpForwardingClient = httpForwardingClient;
            this.tokenCache = tokenCache;
            this.options = options.Value;
        }

        public void Enable()
        {
            httpForwardingClient.Enable();
        }

        public async Task OnAccessTokenChangeAsync(string accessToken)
        {
            await httpForwardingClient.EnsureConnected(accessToken);
        }

        public async Task EnsureConnected()
        {
            var token = await GetToken();
            if (token.NonEmpty())
            {
                await httpForwardingClient.EnsureConnected(token);
            }
        }

        private async Task<string?> GetToken()
        {
            var configuration = options.IdentityClients.First().Value;
            string cacheKey = IdentityModelTokenCacheItem.CalculateCacheKey(configuration);
            var token = await tokenCache.GetAsync(cacheKey);
            return token?.AccessToken;
        }
    }
}
