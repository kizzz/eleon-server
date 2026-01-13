using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Handlers;
using Logging.Module.ErrorHandling.Mappers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Integration;

/// <summary>
/// Integration tests for error handling components working together.
/// </summary>
public class ErrorHandlingIntegrationTests
{
    [Fact]
    public async Task GlobalExceptionHandler_With_Real_Mapper_And_Enricher_Should_Work()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var mapper = new DefaultExceptionMapper(optionsWrapper);
        var enricher = new DefaultErrorEnricher(
            optionsWrapper,
            ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false));
        var problemService = ErrorHandlingTestHelpers.CreateMockProblemDetailsService();
        var logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        
        var handler = new GlobalExceptionHandler(
            problemService, mapper, enricher, logger, environment, optionsWrapper);
        
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new ArgumentException("Invalid argument");

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(400);
        context.Response.Headers["X-Error-Code"].ToString().Should().Be("ELEON-REQ-400");
        await problemService.Received().TryWriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Status == 400 &&
                ctx.ProblemDetails.Extensions.ContainsKey("code") &&
                ctx.ProblemDetails.Extensions["code"].ToString() == "ELEON-REQ-400"));
    }

    [Fact]
    public async Task Full_Flow_With_Custom_Mapper_Should_Use_Custom_Mapping()
    {
        // Arrange
        var customMapper = ErrorHandlingTestHelpers.CreateMockExceptionMapper(ex => (418, "ELEON-TEAPOT", "I'm a teapot"));
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var enricher = ErrorHandlingTestHelpers.CreateMockErrorEnricher();
        var problemService = ErrorHandlingTestHelpers.CreateMockProblemDetailsService();
        var logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        
        var handler = new GlobalExceptionHandler(
            problemService, customMapper, enricher, logger, environment, optionsWrapper);
        
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        context.Response.StatusCode.Should().Be(418);
        context.Response.Headers["X-Error-Code"].ToString().Should().Be("ELEON-TEAPOT");
    }

    [Fact]
    public async Task Full_Flow_With_Custom_Enricher_Should_Add_Custom_Extensions()
    {
        // Arrange
        var customEnricher = ErrorHandlingTestHelpers.CreateMockErrorEnricher((pd, ctx, ex) =>
        {
            pd.Extensions["customField"] = "customValue";
        });
        var mapper = ErrorHandlingTestHelpers.CreateMockExceptionMapper();
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var problemService = ErrorHandlingTestHelpers.CreateMockProblemDetailsService();
        var logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        
        var handler = new GlobalExceptionHandler(
            problemService, mapper, customEnricher, logger, environment, optionsWrapper);
        
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        await problemService.Received().TryWriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Extensions.ContainsKey("customField") &&
                ctx.ProblemDetails.Extensions["customField"].ToString() == "customValue"));
    }
}
