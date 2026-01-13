using FluentAssertions;
using Logging.Module.ErrorHandling.Enrichers;
using Logging.Module.ErrorHandling.Extensions;
using Logging.Module.ErrorHandling.Handlers;
using Logging.Module.ErrorHandling.Mappers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Extensions;

public class ErrorHandlingExtensionsTests
{
    [Fact]
    public void AddExceptionHandling_Should_Register_IProblemDetailsService()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddExceptionHandling(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var problemDetailsService = serviceProvider.GetService<IProblemDetailsService>();
        problemDetailsService.Should().NotBeNull();
    }

    [Fact]
    public void AddExceptionHandling_Should_Register_IExceptionMapper()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddExceptionHandling(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IExceptionMapper>();
        mapper.Should().NotBeNull();
        mapper.Should().BeOfType<DefaultExceptionMapper>();
    }

    [Fact]
    public void AddExceptionHandling_Should_Register_IErrorEnricher()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddExceptionHandling(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var enricher = serviceProvider.GetService<IErrorEnricher>();
        enricher.Should().NotBeNull();
        enricher.Should().BeOfType<DefaultErrorEnricher>();
    }

    [Fact]
    public void AddExceptionHandling_Should_Register_GlobalExceptionHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddExceptionHandling(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handlers = serviceProvider.GetServices<IExceptionHandler>();
        handlers.Should().Contain(h => h is GlobalExceptionHandler);
    }

    [Fact]
    public void AddExceptionHandling_Should_Configure_ErrorHandlingOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ErrorHandling:DefaultHttpStatus", "503" },
                { "ErrorHandling:DefaultAppCode", "TEST-0000" }
            })
            .Build();

        // Act
        services.AddExceptionHandling(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ErrorHandlingOptions>>();
        options.Value.DefaultHttpStatus.Should().Be(503);
        options.Value.DefaultAppCode.Should().Be("TEST-0000");
    }

    [Fact]
    public void UseExceptionHandlingMiddleware_Should_Not_Throw()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        // Act & Assert
        var act = () => app.UseExceptionHandlingMiddleware();
        act.Should().NotThrow();
    }
}
