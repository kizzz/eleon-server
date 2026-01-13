using Common.EventBus.Module;
using Common.EventBus.Module.Options;
using Common.Module.Constants;
using GatewayManagement.Module.Proxies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.GatewayClient.Host.EventBus
{
    public class EventBusConnector : ISingletonDependency
    {
        private readonly DistributedBusResolver distributedBusResolver;
        private readonly IGatewayClientAppService gatewayClientAppService;

        public EventBusConnector(
            DistributedBusResolver distributedBusResolver,
            IGatewayClientAppService gatewayClientAppService)
        {
            this.distributedBusResolver = distributedBusResolver;
            this.gatewayClientAppService = gatewayClientAppService;
        }

        public async Task ConnectBus(EventBusProvider provider, string providerOptionsJson)
        {
            var busOptions = new EventBusOptions()
            {
                Provider = provider,
                ProviderOptionsJson = providerOptionsJson,
            };

            await distributedBusResolver.ConnectEventBus(null, busOptions);

            await gatewayClientAppService.SetGatewayHealthStatus(new GatewayManagement.Module.Proxies.SetGatewayHealthStatusRequestDto()
            {
                HealthStatus = Common.Module.Constants.ServiceHealthStatus.Healthy,
            });
        }
    }
}
