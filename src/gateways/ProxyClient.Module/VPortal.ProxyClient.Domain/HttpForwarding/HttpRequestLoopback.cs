using ProxyManagement.Module.HttpForwarding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Common.Module.Helpers;
using Logging.Module;
using VPortal.ProxyClient.Domain.Auth;

namespace VPortal.ProxyClient.Domain.HttpForwarding
{
    public class HttpRequestLoopback : IHttpRequestLoopback, ITransientDependency
    {
        private static readonly Regex PlaceholderUriRegex = new("http(?:s)?:\\/\\/[^\\/]+");
        private readonly IVportalLogger<HttpRequestLoopback> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public HttpRequestLoopback(
            IVportalLogger<HttpRequestLoopback> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage requestMessage)
        {
            HttpResponseMessage result = null!;
            try
            {
                string selfUrl = configuration["App:SelfUrl"]!;
                Uri placeholderUri = requestMessage.RequestUri!;
                string placeholderUrl = placeholderUri.ToString();

                string loopbackUrl = PlaceholderUriRegex.Replace(placeholderUrl, selfUrl, 1);
                requestMessage.RequestUri = new Uri(loopbackUrl);

                var cert = LicenseHelper.GetCertificate();
                var handler = new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
                    ClientCertificates = { cert },
                };

                var httpClient = httpClientFactory.CreateClient();
                var response = await httpClient.SendAsync(requestMessage);
                response.RequestMessage!.RequestUri = placeholderUri;
                result = response;
            }
            catch (Exception ex)
            {
                logger.Capture(ex);
            }

            return result;
        }
    }
}
