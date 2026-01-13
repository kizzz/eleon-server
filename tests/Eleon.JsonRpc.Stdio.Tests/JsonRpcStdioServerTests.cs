using System.Text.Json.Nodes;
using Eleon.JsonRpc.Stdio;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace Eleon.JsonRpc.Stdio.Tests;

public sealed class JsonRpcStdioServerTests
{
    [Fact]
    public async Task Writes_Response_For_Valid_Request()
    {
        var request = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"ping\"}";
        var input = new StringReader(request);
        var output = new StringWriter();
        var handler = new TestHandler(JsonRpcResponse.Success(JsonValue.Create("pong"), JsonValue.Create(1)));
        var server = new JsonRpcStdioServer(input, output, NullLogger<JsonRpcStdioServer>.Instance);

        await server.RunAsync(handler, CancellationToken.None);

        var responseText = output.ToString().Trim();
        responseText.Should().Contain("\"result\":\"pong\"");
    }

    [Fact]
    public async Task Returns_Parse_Error_On_Invalid_Json()
    {
        var input = new StringReader("not-json");
        var output = new StringWriter();
        var handler = new TestHandler(JsonRpcResponse.Success(null, JsonValue.Create(1)));
        var server = new JsonRpcStdioServer(input, output, NullLogger<JsonRpcStdioServer>.Instance);

        await server.RunAsync(handler, CancellationToken.None);

        var response = output.ToString();
        response.Should().Contain("\"code\":-32700");
    }

    private sealed class TestHandler : IJsonRpcHandler
    {
        private readonly JsonRpcResponse response;

        public TestHandler(JsonRpcResponse response)
        {
            this.response = response;
        }

        public Task<JsonRpcResponse> HandleAsync(JsonRpcRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(response);
        }
    }
}
