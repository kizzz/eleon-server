using Eleon.McpCodexGateway.Host.Stdio;
using FluentAssertions;

namespace Eleon.McpCodexGateway.Host.Stdio.Tests;

public sealed class CommandLineTokenizerTests
{
    [Theory]
    [InlineData("--flag value", new[] { "--flag", "value" })]
    [InlineData("\"quoted value\" tail", new[] { "quoted value", "tail" })]
    [InlineData("--path \"C:\\\\files\\\\example\"", new[] { "--path", "C:\\files\\example" })]
    public void Tokenize_SplitsAndPreservesQuotes(string input, string[] expected)
    {
        var tokens = CommandLineTokenizer.Tokenize(input);

        tokens.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Fact]
    public void Tokenize_ReturnsEmptyArrayWhenInputMissing()
    {
        CommandLineTokenizer.Tokenize(null).Should().BeEmpty();
        CommandLineTokenizer.Tokenize(string.Empty).Should().BeEmpty();
        CommandLineTokenizer.Tokenize("   ").Should().BeEmpty();
    }

    [Theory]
    [InlineData("\\\"quoted\\\"", new[] { "\"quoted\"" })]
    [InlineData("arg1 \"two words\" end\\\\", new[] { "arg1", "two words", "end\\" })]
    public void Tokenize_HandlesEscapedQuotesAndTrailingBackslash(string input, string[] expected)
    {
        var tokens = CommandLineTokenizer.Tokenize(input);

        tokens.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
