using System.Diagnostics;
using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Enrichers;

public class DefaultErrorEnricherTests
{
    private readonly DefaultErrorEnricher _enricher;
    private readonly IHostEnvironment _environment;

    public DefaultErrorEnricherTests()
    {
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        _environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        _enricher = new DefaultErrorEnricher(optionsWrapper, _environment);
    }

    [Fact]
    public void Enrich_Should_Add_TraceId_From_Activity()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var context = ErrorHandlingTestHelpers.CreateHttpContext();
        var activity = ErrorHandlingTestHelpers.SetupActivity("00-trace-id-00");
        
        try
        {
            // Act
            _enricher.Enrich(problemDetails, context, null);

            // Assert
            problemDetails.Extensions.Should().ContainKey("traceId");
            problemDetails.Extensions["traceId"].Should().NotBeNull();
        }
        finally
        {
            activity?.Stop();
        }
    }

    [Fact]
    public void Enrich_Should_Fallback_To_TraceIdentifier_When_No_Activity()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var context = ErrorHandlingTestHelpers.CreateHttpContext();
        var traceId = context.TraceIdentifier;

        // Act
        _enricher.Enrich(problemDetails, context, null);

        // Assert
        problemDetails.Extensions.Should().ContainKey("traceId");
        problemDetails.Extensions["traceId"].Should().Be(traceId);
    }

    [Fact]
    public void Enrich_Should_Add_Instance_From_RequestPath()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var context = ErrorHandlingTestHelpers.CreateHttpContext(path: "/api/test");

        // Act
        _enricher.Enrich(problemDetails, context, null);

        // Assert
        problemDetails.Instance.Should().Be("/api/test");
    }

    [Fact]
    public void Enrich_Should_Add_TenantId_From_Items()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var tenantId = Guid.NewGuid().ToString();
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            items: new Dictionary<string, object?> { { "__TenantId", tenantId } });

        // Act
        _enricher.Enrich(problemDetails, context, null);

        // Assert
        problemDetails.Extensions.Should().ContainKey("tenantId");
        problemDetails.Extensions["tenantId"].Should().Be(tenantId);
    }

    [Fact]
    public void Enrich_Should_Add_TenantId_From_Header()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var tenantId = Guid.NewGuid().ToString();
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            headers: new Dictionary<string, string> { { "X-Tenant-Id", tenantId } });

        // Act
        _enricher.Enrich(problemDetails, context, null);

        // Assert
        problemDetails.Extensions.Should().ContainKey("tenantId");
        problemDetails.Extensions["tenantId"].Should().Be(tenantId);
    }

    [Fact]
    public void Enrich_Should_Add_CorrelationId_From_Items()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var correlationId = Guid.NewGuid().ToString();
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            items: new Dictionary<string, object?> { { "__CorrelationId", correlationId } });

        // Act
        _enricher.Enrich(problemDetails, context, null);

        // Assert
        problemDetails.Extensions.Should().ContainKey("correlationId");
        problemDetails.Extensions["correlationId"].Should().Be(correlationId);
    }

    [Fact]
    public void Enrich_Should_Add_CorrelationId_From_Header()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var correlationId = Guid.NewGuid().ToString();
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            headers: new Dictionary<string, string> { { "X-Correlation-Id", correlationId } });

        // Act
        _enricher.Enrich(problemDetails, context, null);

        // Assert
        problemDetails.Extensions.Should().ContainKey("correlationId");
        problemDetails.Extensions["correlationId"].Should().Be(correlationId);
    }

    [Fact]
    public void Enrich_Should_Add_ExceptionType_In_Development()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var devEnvironment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var enricher = new DefaultErrorEnricher(optionsWrapper, devEnvironment);
        var problemDetails = new ProblemDetails();
        var context = ErrorHandlingTestHelpers.CreateHttpContext();
        var exception = new InvalidOperationException("Test");

        // Act
        enricher.Enrich(problemDetails, context, exception);

        // Assert
        problemDetails.Extensions.Should().ContainKey("exceptionType");
        problemDetails.Extensions["exceptionType"].Should().Be(typeof(InvalidOperationException).FullName);
    }

    [Fact]
    public void Enrich_Should_Not_Add_ExceptionType_In_Production()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var context = ErrorHandlingTestHelpers.CreateHttpContext();
        var exception = new InvalidOperationException("Test");

        // Act
        _enricher.Enrich(problemDetails, context, exception);

        // Assert
        problemDetails.Extensions.Should().NotContainKey("exceptionType");
    }

    [Fact]
    public void Enrich_Should_Handle_Null_ProblemDetails()
    {
        // Arrange
        var context = ErrorHandlingTestHelpers.CreateHttpContext();

        // Act & Assert - should not throw
        _enricher.Enrich(null!, context, null);
    }

    [Fact]
    public void Enrich_Should_Handle_Null_HttpContext()
    {
        // Arrange
        var problemDetails = new ProblemDetails();

        // Act & Assert - should not throw
        _enricher.Enrich(problemDetails, null!, null);
    }

    [Fact]
    public void Enrich_Should_Handle_Null_Exception()
    {
        // Arrange
        var problemDetails = new ProblemDetails();
        var context = ErrorHandlingTestHelpers.CreateHttpContext();

        // Act & Assert - should not throw
        _enricher.Enrich(problemDetails, context, null);
    }
}
