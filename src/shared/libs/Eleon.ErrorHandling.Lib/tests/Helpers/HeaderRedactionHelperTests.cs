using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Helpers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Helpers;

public class HeaderRedactionHelperTests
{
    [Fact]
    public void GetSanitizedHeaders_When_IncludeRequestHeaders_False_Should_Return_Empty()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = false);
        var context = ErrorHandlingTestHelpers.CreateContextWithSensitiveHeaders();
        var headers = context.Request.Headers;

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(headers, options);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetSanitizedHeaders_When_IncludeRequestHeaders_True_Should_Return_Sanitized()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = true);
        var context = ErrorHandlingTestHelpers.CreateContextWithSensitiveHeaders();
        var headers = context.Request.Headers;

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(headers, options);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void GetSanitizedHeaders_Should_Redact_Sensitive_Headers()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = true);
        var context = ErrorHandlingTestHelpers.CreateContextWithSensitiveHeaders();
        var headers = context.Request.Headers;

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(headers, options);

        // Assert
        // Assert - check that sensitive headers are redacted
        result.Should().ContainKey("Authorization");
        result["Authorization"].Should().Be("[Redacted]");
        result.Should().ContainKey("Cookie");
        result["Cookie"].Should().Be("[Redacted]");
        result.Should().ContainKey("X-Api-Key");
        result["X-Api-Key"].Should().Be("[Redacted]");
    }

    [Fact]
    public void GetSanitizedHeaders_Should_Preserve_NonSensitive_Headers()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = true);
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            headers: new Dictionary<string, string>
            {
                { "User-Agent", "TestAgent/1.0" },
                { "Accept", "application/json" }
            });

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(context.Request.Headers, options);

        // Assert
        result.Should().ContainKey("User-Agent");
        result["User-Agent"].Should().Be("TestAgent/1.0");
        result.Should().ContainKey("Accept");
        result["Accept"].Should().Be("application/json");
    }

    [Fact]
    public void GetSanitizedHeaders_Should_Truncate_Long_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o =>
        {
            o.IncludeRequestHeaders = true;
            o.MaxFieldLength = 10;
        });
        var longValue = new string('a', 100);
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            headers: new Dictionary<string, string> { { "Custom-Header", longValue } });

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(context.Request.Headers, options);

        // Assert
        result["Custom-Header"].Should().HaveLength(25); // 10 + "... [TRUNCATED]"
        result["Custom-Header"].Should().EndWith("... [TRUNCATED]");
    }

    [Fact]
    public void GetSanitizedHeaders_Should_Handle_Null_Headers()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = true);

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(null!, options);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetSanitizedHeaders_Should_Be_CaseInsensitive()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = true);
        var context = ErrorHandlingTestHelpers.CreateHttpContext(
            headers: new Dictionary<string, string> { { "AUTHORIZATION", "Bearer token" } });

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(context.Request.Headers, options);

        // Assert
        result["AUTHORIZATION"].Should().Be("[REDACTED]");
    }

    [Fact]
    public void GetSanitizedHeaders_Should_Join_Multiple_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeRequestHeaders = true);
        var context = new DefaultHttpContext();
        context.Request.Headers.Append("Custom-Header", "value1");
        context.Request.Headers.Append("Custom-Header", "value2");

        // Act
        var result = HeaderRedactionHelper.GetSanitizedHeaders(context.Request.Headers, options);

        // Assert
        result["Custom-Header"].Should().Contain("value1");
        result["Custom-Header"].Should().Contain("value2");
    }
}
