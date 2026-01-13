using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Eleon.McpGateway.Module.Test.TestBase;
using FluentAssertions;
using Eleon.McpGateway.Module.Test.TestHelpers;
using Xunit;

namespace Eleon.McpGateway.Module.Test.HttpApi;

[Trait("Category", "Manual")]
public sealed class SsePipelineTests : McpGatewayTestBase
{
    private readonly FakeBackend backend = new();

    public SsePipelineTests()
    {
        RegisterBackends(backend);
    }

    [ManualTestFact]
    public async Task Gateway_ForwardsPostAndStreamsBackendResponses()
    {
        using var sseResponse = await HttpClient.GetAsync("/sse", HttpCompletionOption.ResponseHeadersRead);
        sseResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var reader = new StreamReader(await sseResponse.Content.ReadAsStreamAsync());

        var requestPayload = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","method":"echo"}""")!;
        var postResponse = await HttpClient.PostAsync("/sse", new StringContent(requestPayload.ToJsonString(), Encoding.UTF8, "application/json"));
        postResponse.StatusCode.Should().Be(HttpStatusCode.Accepted);
        backend.SentMessages
            .Where(node => node["method"] != null && node["method"]!.GetValue<string>() == "echo")
            .Should()
            .ContainSingle();

        var backendReply = JsonNode.Parse("""{"jsonrpc":"2.0","id":"1","result":{"ok":true}}""")!;
        backend.Enqueue(backendReply);

        var data = await ReadNextSseEventAsync(reader, TimeSpan.FromSeconds(3));
        data.Should().Be(backendReply.ToJsonString());
    }

    private static async Task<string> ReadNextSseEventAsync(StreamReader reader, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var builder = new StringBuilder();
        while (!cts.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync();
            if (line is null)
            {
                break;
            }

            if (line.StartsWith("data: ", StringComparison.Ordinal))
            {
                builder.Append(line[6..]);
            }
            else if (line.Length == 0 && builder.Length > 0)
            {
                return builder.ToString();
            }
        }

        throw new TimeoutException("Timed out waiting for SSE message.");
    }

}

