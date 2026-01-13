using Common.EventBus.Module.Options;
using GatewayManagement.Module.HttpForwarding;
using GatewayManagement.Module.Proxies;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.GatewayClient.Domain.HttpForwarding;
using VPortal.GatewayClient.Host.EventBus;
using Eleon.Logging.Lib.VportalLogging;

namespace VPortal.GatewayClient.Host.WssLoopback
{
    internal class GatewayHostedService : IHostedService
    {
        private readonly IGatewayClientAppService gatewayClientAppService;
        private readonly HttpForwardingClientConnector httpForwardingClientConnector;
        private readonly GatewayVpnManager gatewayVpnManager;
        private readonly NatsRuntimeManager natsRuntimeManager;
        private readonly EventBusConnector eventBusConnector;
        private readonly IBoundaryLogger boundaryLogger;
        private IBoundaryScope? boundaryScope;
        private Timer? timer = null;

        public GatewayHostedService(
            IGatewayClientAppService gatewayClientAppService,
            HttpForwardingClientConnector httpForwardingClientConnector,
            GatewayVpnManager gatewayVpnManager,
            NatsRuntimeManager natsRuntimeManager,
            EventBusConnector eventBusConnector,
            IBoundaryLogger boundaryLogger)
        {
            this.gatewayClientAppService = gatewayClientAppService;
            this.httpForwardingClientConnector = httpForwardingClientConnector;
            this.gatewayVpnManager = gatewayVpnManager;
            this.natsRuntimeManager = natsRuntimeManager;
            this.eventBusConnector = eventBusConnector;
            this.boundaryLogger = boundaryLogger;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            boundaryScope = boundaryLogger.Begin("HostedService GatewayHostedService");
            string workspaceName = "Default";
            var gateway = await gatewayClientAppService.GetCurrentGatewayWorkspace(workspaceName);

            await gatewayVpnManager.StartAsync(gateway);

            if (gateway.SelfHostEventBus)
            {
                if (gateway.EventBusProvider != Common.Module.Constants.EventBusProvider.NATS)
                {
                    throw new Exception("Self-hosting event bus is only implemented for NATS at the moment.");
                }

                var providerOptions = JsonConvert.DeserializeObject<NatsOptions>(gateway.EventBusProviderOptionsJson);
                await natsRuntimeManager.StartNatsRuntime(providerOptions!);
            }

            await eventBusConnector.ConnectBus(gateway.EventBusProvider, gateway.EventBusProviderOptionsJson);

            timer = new Timer(HealthCheck, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            boundaryScope?.Dispose();
            boundaryScope = null;
            timer?.Change(Timeout.Infinite, 0);
        }

        private async void HealthCheck(object? _)
        {
            var gatewayInfo = await gatewayClientAppService.GetCurrentGateway();

            await gatewayClientAppService.SetGatewayHealthStatus(new GatewayManagement.Module.Proxies.SetGatewayHealthStatusRequestDto()
            {
                HealthStatus = Common.Module.Constants.ServiceHealthStatus.Healthy,
            });

            await Console.Out.WriteLineAsync($"{DateTime.Now}:\tHealthy. Running as {gatewayInfo.Name}.");
        }
    }
}
