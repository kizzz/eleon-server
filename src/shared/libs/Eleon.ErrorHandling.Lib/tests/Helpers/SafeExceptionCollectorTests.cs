using Eleon.ErrorHandling.Lib.Tests.TestHelpers;
using FluentAssertions;
using Logging.Module.ErrorHandling.Helpers;
using Logging.Module.ErrorHandling.Options;
using Xunit;

namespace Eleon.ErrorHandling.Lib.Tests.Helpers;

public class SafeExceptionCollectorTests
{
    [Fact]
    public void Collect_Should_Collect_Exception_Message_Chain()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var inner = new Exception("Inner");
        var outer = new Exception("Outer", inner);

        // Act
        var result = SafeExceptionCollector.Collect(outer, options);

        // Assert
        result.Message.Should().Contain("Outer");
        result.Message.Should().Contain("Inner");
        result.Message.Should().Contain(" => ");
    }

    [Fact]
    public void Collect_Should_Limit_Inner_Exception_Depth()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxInnerExceptionDepth = 3);
        var exception = ErrorHandlingTestHelpers.CreateDeepExceptionChain(10);

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Message.Should().Contain("[Inner exception chain truncated]");
    }

    [Fact]
    public void Collect_Should_Truncate_Message_At_MaxFieldLength()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxFieldLength = 10);
        var longMessage = new string('a', 100);
        var exception = new Exception(longMessage);

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Message.Length.Should().BeLessThanOrEqualTo(25); // 10 + "... [TRUNCATED]"
        result.Message.Should().EndWith("... [TRUNCATED]");
    }

    [Fact]
    public void Collect_Should_Truncate_StackTrace()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxFieldLength = 50);
        var exception = new Exception("Test");

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        // Stack trace can be longer, but should be truncated if exceeds MaxFieldLength * 2
        result.StackTrace.Length.Should().BeLessThanOrEqualTo(125); // 50 * 2 + "... [TRUNCATED]"
    }

    [Fact]
    public void Collect_Should_Collect_Exception_Data()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithData(
            data: new Dictionary<string, object?> { { "Key1", "Value1" }, { "Key2", 123 } });

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Data.Should().ContainKey("Key1");
        result.Data["Key1"].Should().Be("Value1");
        result.Data.Should().ContainKey("Key2");
    }

    [Fact]
    public void Collect_Should_Limit_Dictionary_Entries()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxCollectionItems = 3);
        var data = new Dictionary<string, object?>();
        for (int i = 0; i < 10; i++)
        {
            data[$"Key{i}"] = $"Value{i}";
        }
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithData(data: data);

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Data.Count.Should().BeLessThanOrEqualTo(4); // 3 + 1 for truncation message
        result.Data.Should().ContainKey("[TRUNCATED]");
    }

    [Fact]
    public void Collect_Should_Handle_Null_Exception()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();

        // Act
        var result = SafeExceptionCollector.Collect(null!, options);

        // Assert
        result.Message.Should().BeEmpty();
        result.StackTrace.Should().BeEmpty();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public void Collect_Should_Handle_Exception_With_Null_InnerException()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var exception = new Exception("Test", null);

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Message.Should().Contain("Test");
        result.Message.Should().NotContain(" => ");
    }

    [Fact]
    public void Collect_Should_Handle_Exception_With_Null_Data()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var exception = new Exception("Test");
        // Exception.Data is never null, but can be empty

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public void Collect_Should_Handle_Exception_With_Problematic_Data_Values()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions();
        var problematicObject = new object();
        var exception = ErrorHandlingTestHelpers.CreateExceptionWithData(
            data: new Dictionary<string, object?> { { "Key", problematicObject } });

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert - should not throw, should handle gracefully
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void Collect_Should_Handle_Deep_Exception_Chain()
    {
        // Arrange
        var options = ErrorHandlingTestHelpers.CreateErrorHandlingOptions(o => o.MaxInnerExceptionDepth = 10);
        var exception = ErrorHandlingTestHelpers.CreateDeepExceptionChain(15);

        // Act
        var result = SafeExceptionCollector.Collect(exception, options);

        // Assert
        result.Message.Should().Contain("[Inner exception chain truncated]");
    }
}
