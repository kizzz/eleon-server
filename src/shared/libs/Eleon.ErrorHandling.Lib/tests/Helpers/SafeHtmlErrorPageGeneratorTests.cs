using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Helpers;
using Logging.Module.ErrorHandling.Options;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Helpers;

public class SafeHtmlErrorPageGeneratorTests
{
    [Fact]
    public void Generate_Should_Create_Valid_HTML_Structure()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Test message", "Friendly message", null, null, options, environment);

        // Assert
        html.Should().Contain("<!DOCTYPE html>");
        html.Should().Contain("<html");
        html.Should().Contain("</html>");
    }

    [Fact]
    public void Generate_Should_HTML_Encode_Status_Code_And_Message()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        var message = "<script>alert('xss')</script>";

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", message, null, null, null, options, environment);

        // Assert
        html.Should().NotContain("<script>");
        html.Should().Contain("&lt;script&gt;");
    }

    [Fact]
    public void Generate_Should_HTML_Encode_Stack_Trace()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var stackTrace = "at Test()\n<tag>content</tag>";

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, stackTrace, null, options, environment);

        // Assert
        html.Should().NotContain("<tag>");
        html.Should().Contain("&lt;tag&gt;");
    }

    [Fact]
    public void Generate_Should_HTML_Encode_Dictionary_Keys_And_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var data = new Dictionary<string, object?>
        {
            { "<key>", "<value>" },
            { "normal", "text" }
        };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, null, data, options, environment);

        // Assert
        html.Should().NotContain("<key>");
        html.Should().NotContain("<value>");
        html.Should().Contain("&lt;key&gt;");
        html.Should().Contain("&lt;value&gt;");
    }

    [Fact]
    public void Generate_Should_Include_Exception_Details_In_Development()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var stackTrace = "Stack trace here";
        var data = new Dictionary<string, object?> { { "Key", "Value" } };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, stackTrace, data, options, environment);

        // Assert
        html.Should().Contain("Stack trace here");
        html.Should().Contain("Key");
        html.Should().Contain("Value");
    }

    [Fact]
    public void Generate_Should_Exclude_Exception_Details_In_Production()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        var stackTrace = "Stack trace here";
        var data = new Dictionary<string, object?> { { "Key", "Value" } };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, stackTrace, data, options, environment);

        // Assert
        html.Should().NotContain("Stack trace here");
        html.Should().NotContain("Key");
    }

    [Fact]
    public void Generate_Should_Include_Exception_Details_When_IncludeExceptionDetails_True()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IncludeExceptionDetails = true);
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        var stackTrace = "Stack trace here";

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, stackTrace, null, options, environment);

        // Assert
        html.Should().Contain("Stack trace here");
    }

    [Fact]
    public void Generate_Should_Use_Friendly_Message_When_IsFriendlyErrors_True()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IsFriendlyErrors = true);
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Technical message", "Friendly message", null, null, options, environment);

        // Assert
        html.Should().Contain("Friendly message");
        html.Should().NotContain("Technical message");
    }

    [Fact]
    public void Generate_Should_Use_Detailed_Message_When_IsFriendlyErrors_False()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.IsFriendlyErrors = false);
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Technical message", "Friendly message", null, null, options, environment);

        // Assert
        html.Should().Contain("Technical message");
    }

    [Fact]
    public void Generate_Should_Limit_Collection_Items()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxCollectionItems = 2);
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var data = new Dictionary<string, object?>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            { "Key3", "Value3" },
            { "Key4", "Value4" }
        };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, null, data, options, environment);

        // Assert
        html.Should().Contain("Key1");
        html.Should().Contain("Key2");
        // Should be truncated
        html.Should().Contain("more items");
    }

    [Fact]
    public void Generate_Should_Truncate_Long_Strings()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxFieldLength = 10);
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: false);
        var longMessage = new string('a', 100);

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", longMessage, null, null, null, options, environment);

        // Assert
        html.Should().Contain("... [TRUNCATED]");
    }

    [Fact]
    public void Generate_Should_Handle_Null_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var data = new Dictionary<string, object?> { { "Key", null } };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, null, data, options, environment);

        // Assert - should not throw and should handle null
        html.Should().NotBeNull();
    }

    [Fact]
    public void Generate_Should_Handle_Dictionary_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var dictValue = new Dictionary<string, object?> { { "Nested", "Value" } };
        var data = new Dictionary<string, object?> { { "Key", dictValue } };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, null, data, options, environment);

        // Assert
        html.Should().Contain("Nested");
        html.Should().Contain("Value");
    }

    [Fact]
    public void Generate_Should_Handle_Enumerable_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var environment = ErrorHandlingTestHelpers.CreateMockHostEnvironment(isDevelopment: true);
        var listValue = new List<string> { "Item1", "Item2" };
        var data = new Dictionary<string, object?> { { "Key", listValue } };

        // Act
        var html = SafeHtmlErrorPageGenerator.Generate(
            500, "Error", "Message", null, null, data, options, environment);

        // Assert
        html.Should().Contain("Item1");
        html.Should().Contain("Item2");
    }
}
