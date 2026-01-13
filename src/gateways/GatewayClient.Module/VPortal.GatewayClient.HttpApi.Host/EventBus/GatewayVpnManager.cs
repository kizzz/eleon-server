using Common.EventBus.Module.Options;
using Faker;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using VPortal.Infrastructure.Module.Domain.CliBinding;
using WireGuardNT_PInvoke;
using WireGuardNT_PInvoke.WireGuard;
using WireguardPOC;
using Common.EventBus.Module;
using GatewayManagement.Module.Proxies;
using VPortal.GatewayManagement.Module.Proxies;
using System.Security.Cryptography.X509Certificates;

namespace VPortal.GatewayClient.Host.EventBus
{
    public class GatewayVpnManager : ISingletonDependency
    {
        private readonly IVportalLogger<GatewayVpnManager> logger;
        private readonly WireguardManager wireguardManager;
        private readonly IDistributedEventBus distributedEventBus;
        private readonly IConfiguration configuration;

        public GatewayVpnManager(
            IVportalLogger<GatewayVpnManager> logger,
            WireguardManager wireguardManager,
            IDistributedEventBus distributedEventBus,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.wireguardManager = wireguardManager;
            this.distributedEventBus = distributedEventBus;
            this.configuration = configuration;
        }

        public async Task StartAsync(GatewayWorkspaceDto gateway)
        {

            string adapterName = configuration["Vpn:AdapterName"];
            Guid adapterId = Guid.Parse(configuration["Vpn:AdapterId"]);

            try
            {
                wireguardManager.InitAdapter(adapterId, adapterName);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize WireGuard adapter.", ex);
            }


            var cfg = GetVpnSettings(gateway);

            try
            {
                wireguardManager.SetConfiguration(cfg);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to set VPN configuration.", ex);
            }

            try
            {
                wireguardManager.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to start up the VPN service.", ex);
            }

            await EnsureHttpConnectionOverVpn(cfg.InterfaceAddress.ToString());

        }

        public Task StopAsync()
        {
            wireguardManager.Stop();
            return Task.CompletedTask;
        }

        private async Task EnsureHttpConnectionOverVpn(string selfIp)
        {
            var endpointResponse = await distributedEventBus.RequestAsync<HttpConnectionCheckEndpointGotMsg>(new GetHttpConnectionCheckEndpointMsg());
            string endpoint = endpointResponse.Endpoint.Trim('\\', '/');
            var client = new HttpClient();

            string connectionUrl = $@"https:\\{selfIp}\{endpoint}";
            var response = await client.GetAsync(connectionUrl);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"HTTP connection over VPN was not established. Can not reach {connectionUrl}. Status code {response.StatusCode}.");
            }
        }

        private WgConfig GetVpnSettings(GatewayWorkspaceDto gateway)
        {
            string config = $@"
[Interface]
PrivateKey = {gateway.VpnPrivateKey}
ListenPort = {gateway.VpnListenPort}
Address = {gateway.VpnAddress}/24
DNS = 8.8.8.8, 8.8.4.4

[Peer]
PublicKey = {gateway.VpnServerPublicKey}
Endpoint = eleonsoft.network-protected.com:12345
AllowedIps = {gateway.VpnServerAddress}/32
PersistentKeepalive = 25
";

            return WireguardConfigParser.ParseConfFile(config.Split('\n'));
        }
    }
}


//[Peer]
//PublicKey = { peer.PublicKey}
//AllowedIPs = { peer.AllowedIps}
//Endpoint = { peer.Endpoint}
//PersistentKeepalive = { peer.PersistentKeepalive}