namespace Eleon.JsonRpc.Stdio;

public interface IJsonRpcHandler
{
    Task<JsonRpcResponse> HandleAsync(JsonRpcRequest request, CancellationToken cancellationToken);
}
