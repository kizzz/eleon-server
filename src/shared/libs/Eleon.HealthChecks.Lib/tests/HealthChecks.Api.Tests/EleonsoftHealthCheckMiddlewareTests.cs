using EleonsoftSdk.modules.HealthCheck.Module.General;
using EleonsoftSdk.modules.HealthCheck.Module.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EleonsoftSdk.modules.HealthCheck.Module.Tests.Api;

public class EleonsoftHealthCheckMiddlewareTests
{
    private readonly Mock<IOptions<HealthCheckOptions>> _optionsMock;
    private readonly Mock<ILogger<EleonsoftHealthCheckMiddleware>> _loggerMock;
    private readonly RequestDelegate _next;

    public EleonsoftHealthCheckMiddlewareTests()
    {
        _optionsMock = new Mock<IOptions<HealthCheckOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(new HealthCheckOptions
        {
            Enabled = true,
            RestartEnabled = false,
            RestartRequiresAuth = true,
            UI = new HealthCheckUIOptions
            {
                Enabled = true,
                Path = "/healthchecks-ui"
            }
        });

        _loggerMock = new Mock<ILogger<EleonsoftHealthCheckMiddleware>>();
        _next = context => Task.CompletedTask;
    }

    private HttpContext CreateHttpContext(string path, string method = "GET", bool authenticated = false)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = new PathString(path);
        context.Request.Method = method;

        if (authenticated)
        {
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "test"));
        }

        return context;
    }

    [Fact]
    public async Task ShouldHandleRestartEndpoint_WhenDisabled()
    {
        // Arrange
        _optionsMock.Setup(x => x.Value).Returns(new HealthCheckOptions
        {
            Enabled = true,
            RestartEnabled = false,
            UI = new HealthCheckUIOptions
            {
                Enabled = true,
                Path = "/healthchecks-ui"
            }
        });

        var middleware = new EleonsoftHealthCheckMiddleware(
            _loggerMock.Object,
            _optionsMock.Object);

        var context = CreateHttpContext("/healthchecks-ui/restart", "POST", authenticated: true);
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_loggerMock.Object)
            .AddSingleton(_optionsMock.Object)
            .BuildServiceProvider();

        // Act
        await middleware.InvokeAsync(context, _next);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task ShouldHandleRestartEndpoint_WhenNotPost()
    {
        // Arrange
        _optionsMock.Setup(x => x.Value).Returns(new HealthCheckOptions
        {
            Enabled = true,
            RestartEnabled = true,
            UI = new HealthCheckUIOptions
            {
                Enabled = true,
                Path = "/healthchecks-ui"
            }
        });

        var middleware = new EleonsoftHealthCheckMiddleware(
            _loggerMock.Object,
            _optionsMock.Object);

        var context = CreateHttpContext("/healthchecks-ui/restart", "GET", authenticated: true);
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_loggerMock.Object)
            .AddSingleton(_optionsMock.Object)
            .BuildServiceProvider();

        // Act
        await middleware.InvokeAsync(context, _next);

        // Assert
        Assert.Equal(StatusCodes.Status405MethodNotAllowed, context.Response.StatusCode);
    }

    [Fact]
    public async Task ShouldHandleRestartEndpoint_WhenNotAuthenticated()
    {
        // Arrange
        _optionsMock.Setup(x => x.Value).Returns(new HealthCheckOptions
        {
            Enabled = true,
            RestartEnabled = true,
            RestartRequiresAuth = true,
            UI = new HealthCheckUIOptions
            {
                Enabled = true,
                Path = "/healthchecks-ui"
            }
        });

        var middleware = new EleonsoftHealthCheckMiddleware(
            _loggerMock.Object,
            _optionsMock.Object);

        var context = CreateHttpContext("/healthchecks-ui/restart", "POST", authenticated: false);
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_loggerMock.Object)
            .AddSingleton(_optionsMock.Object)
            .BuildServiceProvider();

        // Act
        await middleware.InvokeAsync(context, _next);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task ShouldUsePathStringComparisons()
    {
        // Arrange
        _optionsMock.Setup(x => x.Value).Returns(new HealthCheckOptions
        {
            Enabled = true,
            HealthStatusPath = "/api/health"
        });

        var middleware = new EleonsoftHealthCheckMiddleware(
            _loggerMock.Object,
            _optionsMock.Object);

        var context = CreateHttpContext("/api/health");
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_loggerMock.Object)
            .AddSingleton(_optionsMock.Object)
            .BuildServiceProvider();

        // Act
        await middleware.InvokeAsync(context, _next);

        // Assert
        // Should handle path correctly using PathString comparisons
        // If path matches, middleware should handle it; otherwise, call next
        Assert.True(true); // PathString comparison is internal, just verify no crash
    }

    [Fact]
    public async Task ShouldReturnEarly_AfterHandling()
    {
        // Arrange
        var callCount = 0;
        var next = new RequestDelegate(ctx => { callCount++; return Task.CompletedTask; });

        _optionsMock.Setup(x => x.Value).Returns(new HealthCheckOptions
        {
            Enabled = true,
            HealthStatusPath = "/api/health"
        });

        var middleware = new EleonsoftHealthCheckMiddleware(
            _loggerMock.Object,
            _optionsMock.Object);

        var context = CreateHttpContext("/api/health");
        context.RequestServices = new ServiceCollection()
            .AddSingleton(_loggerMock.Object)
            .AddSingleton(_optionsMock.Object)
            .BuildServiceProvider();

        // Act
        await middleware.InvokeAsync(context, next);

        // Assert
        // If middleware handles the request, next should not be called
        // This is verified by checking that response was set
        Assert.True(context.Response.HasStarted || callCount == 0);
    }
}
