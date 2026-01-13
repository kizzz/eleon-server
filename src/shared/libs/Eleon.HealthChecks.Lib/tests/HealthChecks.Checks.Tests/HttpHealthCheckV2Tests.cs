using EleonsoftSdk.modules.HealthCheck.Module.Checks.Http;
using EleonsoftSdk.modules.HealthCheck.Module.Checks.HttpCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Checks;

public class HttpHealthCheckV2Tests
{
    private readonly Mock<ILogger<HttpHealthCheckV2>> _loggerMock;

    public HttpHealthCheckV2Tests()
    {
        _loggerMock = new Mock<ILogger<HttpHealthCheckV2>>();
    }

    [Fact]
    public async Task ShouldHandleEmptyUrlList()
    {
        // Arrange
        var options = Options.Create(new HttpHealthCheckOptions { Urls = new List<HealthCheckUrl>() });
        var httpClientFactory = new Mock<IHttpClientFactory>();
        var check = new HttpHealthCheckV2(httpClientFactory.Object, options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(0, result.Data["http.checks_count"]);
    }

    [Fact]
    public async Task ShouldHandleHttpRequestException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(handler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl { Name = "test", Url = "https://example.com" }
            }
        });
        var check = new HttpHealthCheckV2(httpClientFactory.Object, options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ShouldHandleTaskCanceledException()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        var httpClient = new HttpClient(handler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl { Name = "test", Url = "https://example.com" }
            }
        });
        var check = new HttpHealthCheckV2(httpClientFactory.Object, options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Should handle cancellation gracefully
    }

    [Fact]
    public async Task ShouldHandleStatusCodeValidation()
    {
        // Arrange
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var httpClient = new HttpClient(handler.Object);
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var options = Options.Create(new HttpHealthCheckOptions
        {
            Urls = new List<HealthCheckUrl>
            {
                new HealthCheckUrl
                {
                    Name = "test",
                    Url = "https://example.com",
                    GoodStatusCodes = new List<int> { 200 }
                }
            }
        });
        var check = new HttpHealthCheckV2(httpClientFactory.Object, options, _loggerMock.Object);

        // Act
        var result = await check.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
    }
}
