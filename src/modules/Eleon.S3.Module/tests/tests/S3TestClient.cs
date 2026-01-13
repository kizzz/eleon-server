using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace EleonS3.Test.Tests;
public static class S3TestClient
{
    public static IAmazonS3 Create(IConfiguration config)
    {
        var section = config.GetSection("S3");
        var endpoint = section["Endpoint"];
        var region = section["Region"];
        if (string.IsNullOrWhiteSpace(endpoint) && string.IsNullOrWhiteSpace(region))
        {
            throw new InvalidOperationException("S3 test configuration missing Endpoint or Region.");
        }

        var credentials = new BasicAWSCredentials(
            section["AccessKey"],
            section["SecretKey"]
        );
        var amazonConfig = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true,
            UseHttp = false,
            AuthenticationRegion = region,
        };
        if (!string.IsNullOrWhiteSpace(region))
        {
            amazonConfig.RegionEndpoint = RegionEndpoint.GetBySystemName(region);
        }
        if (bool.TryParse(section["UseHttp"], out var useHttp))
        {
            amazonConfig.UseHttp = useHttp;
        }
        if (bool.TryParse(section["ForcePathStyle"], out var forcePathStyle))
        {
            amazonConfig.ForcePathStyle = forcePathStyle;
        }
        amazonConfig.HttpClientFactory = new InsecureHttpClientFactory();
        return new AmazonS3Client(credentials, amazonConfig);
    }

    private sealed class InsecureHttpClientFactory : Amazon.Runtime.HttpClientFactory
    {
        public override HttpClient CreateHttpClient(IClientConfig clientConfig)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return new HttpClient(handler, disposeHandler: true);
        }

        public override bool UseSDKHttpClientCaching(IClientConfig clientConfig) => false;

        public override bool DisposeHttpClientsAfterUse(IClientConfig clientConfig) => true;
    }
}
