using System.Diagnostics;
using Eleon.McpGateway.Host.Sse;
using FluentAssertions;

namespace Eleon.McpGateway.Host.Sse.Tests;

public sealed class ProcessLogFormatterTests
{
    [Fact]
    public void Format_QuotesArgumentsWithSpaces()
    {
        var startInfo = new ProcessStartInfo("dotnet");
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--path");
        startInfo.ArgumentList.Add("C:\\Program Files\\My App");

        var formatted = ProcessLogFormatter.Format(startInfo);

        formatted.Should().Be("dotnet run --path \"C:\\Program Files\\My App\"");
    }
}
