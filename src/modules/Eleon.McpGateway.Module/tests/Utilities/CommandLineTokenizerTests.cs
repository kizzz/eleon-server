using Eleon.Mcp.Infrastructure.Utilities;
using FluentAssertions;

namespace Eleon.McpGateway.Module.Test.Utilities;

public sealed class CommandLineTokenizerTests
{
    [Theory]
    [InlineData("--flag value", new[] { "--flag", "value" })]
    [InlineData("\"quoted value\" tail", new[] { "quoted value", "tail" })]
    [InlineData("--auth-session last --no-key", new[] { "--auth-session", "last", "--no-key" })]
    [InlineData("--path \"C:\\\\files\\\\example\"", new[] { "--path", "C:\\files\\example" })]
    public void Tokenize_SplitsAndPreservesQuotes(string input, string[] expected)
    {
        var tokens = CommandLineTokenizer.Tokenize(input);

        tokens.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
    }
}

