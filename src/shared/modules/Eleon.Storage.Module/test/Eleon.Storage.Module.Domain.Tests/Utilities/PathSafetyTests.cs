using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Eleon.Storage.Module.Domain.Tests.Utilities;

/// <summary>
/// Tests for path safety and sanitization to prevent directory traversal and path escaping.
/// </summary>
public class PathSafetyTests
{
    [Theory]
    [InlineData("../x")]
    [InlineData("..\\x")]
    [InlineData("folder/../x")]
    [InlineData("folder\\..\\x")]
    [InlineData("../../etc/passwd")]
    [InlineData("..\\..\\windows\\system32")]
    public void IsPathSafe_WithTraversalSequence_Should_ReturnFalse(string path)
    {
        // Act
        var result = StorageTestHelpers.IsPathSafe(path);

        // Assert
        result.Should().BeFalse($"Path '{path}' should be rejected as unsafe");
    }

    [Theory]
    [InlineData("/absolute/path")]
    [InlineData("\\absolute\\path")]
    [InlineData("C:\\absolute\\path")]
    [InlineData("C:/absolute/path")]
    public void IsPathSafe_WithAbsolutePath_Should_ReturnFalse(string path)
    {
        // Act
        var result = StorageTestHelpers.IsPathSafe(path);

        // Assert
        result.Should().BeFalse($"Absolute path '{path}' should be rejected");
    }

    [Theory]
    [InlineData("folder/subfolder/file.txt")]
    [InlineData("folder\\subfolder\\file.txt")]
    [InlineData("file.txt")]
    [InlineData("folder/file-name.txt")]
    [InlineData("folder/sub-folder/file_name.txt")]
    public void IsPathSafe_WithValidRelativePath_Should_ReturnTrue(string path)
    {
        // Act
        var result = StorageTestHelpers.IsPathSafe(path);

        // Assert
        result.Should().BeTrue($"Valid relative path '{path}' should be accepted");
    }

    [Theory]
    [InlineData("folder\\sub/file.txt")] // Mixed separators
    [InlineData("folder/sub\\file.txt")]
    public void IsPathSafe_WithMixedSeparators_Should_ReturnFalse(string path)
    {
        // Act
        var result = StorageTestHelpers.IsPathSafe(path);

        // Assert
        // Mixed separators should be rejected or normalized - verify current behavior
        // This may need adjustment based on actual implementation
        result.Should().BeFalse($"Mixed separators in '{path}' should be rejected");
    }

    [Theory]
    [InlineData("тест.txt")] // Cyrillic
    [InlineData("测试.txt")] // Chinese
    [InlineData("テスト.txt")] // Japanese
    [InlineData("file with spaces.txt")]
    public void IsPathSafe_WithUnicodeAndSpaces_Should_ReturnTrue(string path)
    {
        // Act
        var result = StorageTestHelpers.IsPathSafe(path);

        // Assert
        result.Should().BeTrue($"Unicode path '{path}' should be handled correctly");
    }

    [Theory]
    [InlineData("/base/path", "/base/path/file.txt", true)]
    [InlineData("/base/path", "/base/path/sub/file.txt", true)]
    [InlineData("/base/path", "/base/path/../other/file.txt", false)] // Escapes base
    [InlineData("C:\\base\\path", "C:\\base\\path\\file.txt", true)]
    [InlineData("C:\\base\\path", "C:\\other\\file.txt", false)]
    public void IsPathContainedIn_Should_VerifyContainment(string basePath, string fullPath, bool expected)
    {
        // Act
        var result = StorageTestHelpers.IsPathContainedIn(fullPath, basePath);

        // Assert
        result.Should().Be(expected, 
            $"Path '{fullPath}' should {(expected ? "be" : "not be")} contained in '{basePath}'");
    }

    [Fact]
    public void IsPathContainedIn_WithNullPaths_Should_ReturnFalse()
    {
        // Act & Assert
        StorageTestHelpers.IsPathContainedIn(null, "/base").Should().BeFalse();
        StorageTestHelpers.IsPathContainedIn("/path", null).Should().BeFalse();
        StorageTestHelpers.IsPathContainedIn(null, null).Should().BeFalse();
    }
}
