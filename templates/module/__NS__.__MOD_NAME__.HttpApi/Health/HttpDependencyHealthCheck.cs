using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace __NS__.__MOD_NAME__.HttpApi.Health;

public sealed class HttpDependencyHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _factory;
    private readonly string _url;

    public HttpDependencyHealthCheck(IHttpClientFactory factory, string url)
    {
        _factory = factory;
        _url = url;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _factory.CreateClient(nameof(HttpDependencyHealthCheck));
            using var resp = await client.GetAsync(_url, cancellationToken);
            if (resp.IsSuccessStatusCode)
                return HealthCheckResult.Healthy($"HTTP OK: {_url} => {(int)resp.StatusCode}");
            return HealthCheckResult.Unhealthy($"HTTP BAD: {_url} => {(int)resp.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"HTTP FAIL: {_url}", ex);
        }
    }
}
