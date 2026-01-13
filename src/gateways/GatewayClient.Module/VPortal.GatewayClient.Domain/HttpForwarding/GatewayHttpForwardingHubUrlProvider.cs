using GatewayManagement.Module.HttpForwarding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.GatewayClient.Domain.HttpForwarding
{
    public class GatewayHttpForwardingHubUrlProvider : IHttpForwardingHubUrlProvider, ITransientDependency
    {
        private readonly IConfiguration configuration;

        public GatewayHttpForwardingHubUrlProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetHttpForwardingHubUrl() => $"{configuration["App:HostUrl"]}/{HttpForwardingRemoteConsts.HttpForwardingHubRoute}";
    }
}
