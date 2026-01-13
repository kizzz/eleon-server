using Eleon.McpCodexGateway.Host.Stdio;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.McpCodexGateway.Host.Stdio.Tests;

public sealed class ErrorStreamForwarderTests
{
    [Fact]
    public async Task ForwardAsync_PrefixesEachLineAndFlushes()
    {
        using var source = new StringReader("first\nsecond\n");
        using var destination = new StringWriter();
        var forwarder = new ErrorStreamForwarder(NullLogger<ErrorStreamForwarder>.Instance);

        await forwarder.ForwardAsync(source, destination, CancellationToken.None);

        var lines = destination.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        lines.Should().BeEquivalentTo(new[] { "[codex] first", "[codex] second" }, opts => opts.WithStrictOrdering());
    }

    [Fact]
    public async Task ForwardAsync_StopsWhenCancelledBeforeRead()
    {
        using var source = new StringReader("should-not-be-read");
        using var destination = new StringWriter();
        var forwarder = new ErrorStreamForwarder(NullLogger<ErrorStreamForwarder>.Instance);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await forwarder.ForwardAsync(source, destination, cts.Token);

        destination.ToString().Should().BeEmpty();
    }
}
