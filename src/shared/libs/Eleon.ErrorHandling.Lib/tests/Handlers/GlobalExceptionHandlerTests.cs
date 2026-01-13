using System.Net.Mime;
using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Handlers;
using Logging.Module.ErrorHandling.Mappers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Handlers;

public class GlobalExceptionHandlerTests
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IExceptionMapper _exceptionMapper;
    private readonly IErrorEnricher _errorEnricher;
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;
    private readonly IOptions<ErrorHandlingOptions> _options;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _problemDetailsService = ErrorHandlingTestHelpers.CreateMockProblemDetailsService();
        _exceptionMapper = ErrorHandlingTestHelpers.CreateMockExceptionMapper();
        _errorEnricher = ErrorHandlingTestHelpers.CreateMockErrorEnricher();
        _logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
        _environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        _options = ErrorHandlingTestHelpers.CreateOptions(ErrorHandlingTestHelpers.CreateErrorHandlingOptions());
        
        _handler = new GlobalExceptionHandler(
            _problemDetailsService,
            _exceptionMapper,
            _errorEnricher,
            _logger,
            _environment,
            _options);
    }

    [Fact]
    public async Task TryHandleAsync_When_Response_HasStarted_Should_Return_False()
    {
        // Arrange
        var context = ErrorHandlingTestHelpers.CreateHttpContext();
        // Simulate response has started by writing to it
        await context.Response.WriteAsync("Started", CancellationToken.None);
        var exception = new Exception("Test");

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TryHandleAsync_Should_Set_HTTP_Status_Code()
    {
        // Arrange
        var mapper = ErrorHandlingTestHelpers.CreateMockExceptionMapper(ex => (404, "ELEON-404", "Not Found"));
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, mapper, _errorEnricher, _logger, _environment, _options);
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task TryHandleAsync_Should_Set_XErrorCode_Header()
    {
        // Arrange
        var mapper = ErrorHandlingTestHelpers.CreateMockExceptionMapper(ex => (500, "ELEON-500", "Error"));
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, mapper, _errorEnricher, _logger, _environment, _options);
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        context.Response.Headers.Should().ContainKey("X-Error-Code");
        context.Response.Headers["X-Error-Code"].ToString().Should().Be("ELEON-500");
    }

    [Fact]
    public async Task TryHandleAsync_Should_Log_Exception()
    {
        // Arrange
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        _logger.Received().LogError(
            exception,
            Arg.Is<string>(s => s.Contains("Unhandled exception")),
            Arg.Any<object[]>());
    }

    [Fact]
    public async Task TryHandleAsync_Should_Use_ProblemDetailsService_For_JSON()
    {
        // Arrange
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await _problemDetailsService.Received().TryWriteAsync(
            Arg.Any<ProblemDetailsContext>());
    }

    [Fact]
    public async Task TryHandleAsync_Should_Return_HTML_For_Browser_Requests()
    {
        // Arrange
        var context = ErrorHandlingTestHelpers.CreateHtmlRequestContext();
        var exception = new Exception("Test");
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.EnableHtmlErrorPages = true);
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, _exceptionMapper, _errorEnricher, _logger, _environment, optionsWrapper);

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        context.Response.ContentType.Should().Be(MediaTypeNames.Text.Html);
    }

    [Fact]
    public async Task TryHandleAsync_Should_Enrich_ProblemDetails()
    {
        // Arrange
        var enricher = ErrorHandlingTestHelpers.CreateMockErrorEnricher((pd, ctx, ex) =>
        {
            pd.Extensions["custom"] = "value";
        });
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, _exceptionMapper, enricher, _logger, _environment, _options);
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        enricher.Received().Enrich(Arg.Any<ProblemDetails>(), context, exception);
    }

    [Fact]
    public async Task TryHandleAsync_Should_Include_Exception_Details_In_Development()
    {
        // Arrange
        var devEnvironment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, _exceptionMapper, _errorEnricher, _logger, devEnvironment, _options);
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        await _problemDetailsService.Received().TryWriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx => 
                ctx.ProblemDetails.Extensions.ContainsKey("exceptionMessage")));
    }

    [Fact]
    public async Task TryHandleAsync_Should_Exclude_Exception_Details_In_Production()
    {
        // Arrange
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        await _problemDetailsService.Received().TryWriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx => 
                !ctx.ProblemDetails.Extensions.ContainsKey("exceptionMessage")));
    }

    [Fact]
    public async Task TryHandleAsync_Should_Use_Friendly_Message_When_IsFriendlyErrors_True()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IsFriendlyErrors = true);
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, _exceptionMapper, _errorEnricher, _logger, _environment, optionsWrapper);
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithFriendlyMessage("Friendly message");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        await _problemDetailsService.Received().TryWriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx => 
                ctx.ProblemDetails.Detail != null && ctx.ProblemDetails.Detail.Contains("Friendly")));
    }

    [Fact]
    public async Task TryHandleAsync_Should_Handle_Exceptions_During_Handling()
    {
        // Arrange
        var problemService = ErrorHandlingTestHelpers.CreateMockProblemDetailsService();
        problemService.When(x => x.TryWriteAsync(Arg.Any<ProblemDetailsContext>()))
            .Throw(new Exception("Handler exception"));
        var handler = new GlobalExceptionHandler(
            problemService, _exceptionMapper, _errorEnricher, _logger, _environment, _options);
        var context = ErrorHandlingTestHelpers.CreateJsonRequestContext();
        var exception = new Exception("Test");

        // Act
        var result = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _logger.Received().LogCritical(
            Arg.Any<Exception>(),
            Arg.Is<string>(s => s.Contains("An error occurred while handling an exception")));
    }

    [Fact]
    public async Task TryHandleAsync_Should_Respect_EnableHtmlErrorPages_False()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.EnableHtmlErrorPages = false);
        var optionsWrapper = ErrorHandlingTestHelpers.CreateOptions(options);
        var handler = new GlobalExceptionHandler(
            _problemDetailsService, _exceptionMapper, _errorEnricher, _logger, _environment, optionsWrapper);
        var context = ErrorHandlingTestHelpers.CreateHtmlRequestContext();
        var exception = new Exception("Test");

        // Act
        await handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        context.Response.ContentType.Should().NotBe(MediaTypeNames.Text.Html);
        await _problemDetailsService.Received().TryWriteAsync(
            Arg.Any<ProblemDetailsContext>());
    }
}
