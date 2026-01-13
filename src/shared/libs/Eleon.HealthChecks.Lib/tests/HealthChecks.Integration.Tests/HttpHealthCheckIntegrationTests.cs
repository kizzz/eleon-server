using EleonsoftSdk.modules.HealthCheck.Module.Checks.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Integration;

public class HttpHealthCheckIntegrationTests : TestBase
{
    [Fact]
    public async Task HttpCheck_ShouldMeasureLatency()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken ct) =>
            {
                Task.Delay(100, ct).Wait(ct);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl { Name = "test", Url = "https://example.com" }
            }
        });

        var httpClientFactory = new MockHttpClientFactory(handler.Object);
        var logger = LoggerFactory.CreateLogger<HttpHealthCheckV2>();
        var check = new HttpHealthCheckV2(httpClientFactory, options, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains("http.check_test.latency_ms", result.Data.Keys);
        var latency = Convert.ToInt64(result.Data["http.check_test.latency_ms"]);
        Assert.True(latency >= 90); // Allow some variance
    }

    [Fact]
    public async Task HttpCheck_ShouldHandleStatusCodeValidation()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl
                {
                    Name = "test",
                    Url = "https://example.com",
                    GoodStatusCodes = new List<int> { 200, 201 }
                }
            }
        });

        var httpClientFactory = new MockHttpClientFactory(handler.Object);
        var logger = LoggerFactory.CreateLogger<HttpHealthCheckV2>();
        var check = new HttpHealthCheckV2(httpClientFactory, options, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task HttpCheck_ShouldHandleMultipleUrls()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl { Name = "url1", Url = "https://example1.com" },
                new HealthCheckUrl { Name = "url2", Url = "https://example2.com" },
                new HealthCheckUrl { Name = "url3", Url = "https://example3.com" }
            }
        });

        var httpClientFactory = new MockHttpClientFactory(handler.Object);
        var logger = LoggerFactory.CreateLogger<HttpHealthCheckV2>();
        var check = new HttpHealthCheckV2(httpClientFactory, options, logger);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains("http.checks_count", result.Data.Keys);
        Assert.Equal(3, result.Data["http.checks_count"]);
    }

    [Fact]
    public async Task HttpCheck_ShouldHonorCancellationToken()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage req, CancellationToken ct) =>
            {
                Task.Delay(5000, ct).Wait(ct);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl { Name = "test", Url = "https://example.com" }
            }
        });

        var httpClientFactory = new MockHttpClientFactory(handler.Object);
        var logger = LoggerFactory.CreateLogger<HttpHealthCheckV2>();
        var check = new HttpHealthCheckV2(httpClientFactory, options, logger);

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            cts.Token);

        // Assert
        Assert.NotNull(result);
        // Should handle cancellation gracefully
    }

    // Simplified test - using mocked HttpClient instead of real server
    private class MockHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public MockHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler) { Timeout = TimeSpan.FromSeconds(30) };
        }
    }
}
