using ProxyManagement.Module.HttpForwarding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.ProxyClient.Domain.HttpForwarding
{
    public class ProxyHttpForwardingHubUrlProvider : IHttpForwardingHubUrlProvider, ITransientDependency
    {
        private readonly IConfiguration configuration;

        public ProxyHttpForwardingHubUrlProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetHttpForwardingHubUrl() => $"{configuration["App:HostUrl"]}/{HttpForwardingRemoteConsts.HttpForwardingHubRoute}";
    }
}
