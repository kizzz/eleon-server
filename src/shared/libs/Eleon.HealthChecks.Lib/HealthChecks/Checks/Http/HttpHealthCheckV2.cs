using EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace EleonsoftSdk.modules.HealthCheck.Module.Checks.Http;

/// <summary>
/// HTTP health check implementing IHealthCheck.
/// Uses HttpClientFactory, honors CancellationToken, returns structured observations.
/// </summary>
public class HttpHealthCheckV2 : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpHealthCheckOptions _options;
    private readonly ILogger<HttpHealthCheckV2> _logger;

    public HttpHealthCheckV2(
        IHttpClientFactory httpClientFactory,
        IOptions<HttpHealthCheckOptions> options,
        ILogger<HttpHealthCheckV2> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_options.Urls == null || _options.Urls.Count == 0)
        {
            return HealthCheckResult.Healthy("No HTTP checks configured", new Dictionary<string, object>
            {
                ["http.checks_count"] = 0
            });
        }

        var results = new List<UrlCheckResult>();
        var sw = Stopwatch.StartNew();

        foreach (var url in _options.Urls)
        {
            var result = new UrlCheckResult { Name = url.Name, Url = url.Url };
            var urlSw = Stopwatch.StartNew();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var client = _httpClientFactory.CreateClient(HttpHealthCheck.DefaultHealthCheckClientName);
                using var response = await client.GetAsync(url.Url, cancellationToken);
                
                urlSw.Stop();
                result.Success = true;
                result.StatusCode = (int)response.StatusCode;
                result.LatencyMs = urlSw.ElapsedMilliseconds;

                // Check if status code is in good list
                if (!url.GoodStatusCodes.Contains(result.StatusCode))
                {
                    result.Success = false;
                    result.Error = $"Unexpected status code {result.StatusCode}";
                }

                // Optionally read small content
                if (response.Content.Headers.ContentLength.HasValue && 
                    response.Content.Headers.ContentLength.Value < 1024)
                {
                    result.Content = await response.Content.ReadAsStringAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                result.Success = false;
                result.Error = "Operation cancelled";
                _logger.LogWarning("HTTP check cancelled for {Url}", url.Url);
            }
            catch (Exception ex)
            {
                urlSw.Stop();
                result.Success = false;
                result.Error = $"{ex.GetType().Name}: {ex.Message}";
                _logger.LogWarning(ex, "HTTP check failed for {Url}", url.Url);
            }
            finally
            {
                result.LatencyMs = urlSw.ElapsedMilliseconds;
                results.Add(result);
            }
        }

        sw.Stop();

        var total = results.Count;
        var ok = results.Count(r => r.Success);
        var failed = total - ok;

        var data = new Dictionary<string, object>
        {
            ["http.latency_ms"] = sw.ElapsedMilliseconds,
            ["http.checks_total"] = total,
            ["http.checks_ok"] = ok,
            ["http.checks_failed"] = failed
        };

        // Add per-URL data
        foreach (var r in results)
        {
            var prefix = $"http.url_{r.Name}";
            data[$"{prefix}.success"] = r.Success;
            data[$"{prefix}.status_code"] = r.StatusCode;
            data[$"{prefix}.latency_ms"] = r.LatencyMs;

            if (!r.Success && !string.IsNullOrEmpty(r.Error))
            {
                data[$"{prefix}.error"] = r.Error;
            }
        }

        if (_options.IgnoreSsl)
        {
            data["http.ignore_ssl_warning"] = "SSL validation is disabled";
        }

        if (failed == 0)
        {
            return HealthCheckResult.Healthy(
                $"All {total} HTTP check(s) passed",
                data);
        }

        if (ok > 0)
        {
            return HealthCheckResult.Degraded(
                $"{ok} of {total} HTTP check(s) passed, {failed} failed",
                data: data);
        }

        return HealthCheckResult.Unhealthy(
            $"All {total} HTTP check(s) failed",
            data: data);
    }

    private sealed class UrlCheckResult
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public long LatencyMs { get; set; }
        public string? Error { get; set; }
        public string? Content { get; set; }
    }
}
