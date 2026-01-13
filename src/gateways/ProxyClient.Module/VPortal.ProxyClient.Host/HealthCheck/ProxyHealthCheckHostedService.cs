using ProxyManagement.Module.HttpForwarding;
using ProxyManagement.Module.Proxies;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.ProxyClient.Domain.HttpForwarding;
using Eleon.Logging.Lib.VportalLogging;

namespace VPortal.ProxyClient.Host.WssLoopback
{
    internal class ProxyHealthCheckHostedService : IHostedService
    {
        private readonly IProxyClientAppService proxyClientAppService;
        private readonly HttpForwardingClientConnector httpForwardingClientConnector;
        private readonly IBoundaryLogger boundaryLogger;
        private IBoundaryScope? boundaryScope;
        private Timer? timer = null;

        public ProxyHealthCheckHostedService(
            IProxyClientAppService proxyClientAppService,
            HttpForwardingClientConnector httpForwardingClientConnector,
            IBoundaryLogger boundaryLogger)
        {
            this.proxyClientAppService = proxyClientAppService;
            this.httpForwardingClientConnector = httpForwardingClientConnector;
            this.boundaryLogger = boundaryLogger;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            boundaryScope = boundaryLogger.Begin("HostedService ProxyHealthCheckHostedService");
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
            var proxyInfo = await proxyClientAppService.GetProxyByLoggedImpersonation();
            if (proxyInfo.Protocol == Common.Module.Constants.ProxyProtocol.WSS)
            {
                httpForwardingClientConnector.Enable();
                await httpForwardingClientConnector.EnsureConnected();
            }

            await Console.Out.WriteLineAsync($"{DateTime.Now}:\tHealthy. Running as {proxyInfo.Name}.");
        }
    }
}
