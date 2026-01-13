# Eleon MCP Gateway (SSE)

ASP.NET Core host that exposes MCP over Server-Sent Events and forwards JSON-RPC traffic to stdio MCP backends. It now ships with two backends out of the box:

- `codex-stdio` → `Eleon.McpCodexGateway.Host.Stdio` (Codex CLI)
- `ssh-stdio`  → `Eleon.McpSshGateway.Host.Stdio` (SSH tool suite)

Additional backends can be registered by implementing `IMcpBackend`.

## How it works

- `GET <basePath>/{backend?}` keeps an SSE stream open (`text/event-stream`) and forwards the chosen backend’s output as `event: message` frames. One active SSE connection per backend is enforced.
- `POST <basePath>/{backend?}` accepts JSON-RPC envelopes and forwards them to the backend stdin. If `{backend}` is omitted, the default backend is used (`codex-stdio`).
- The gateway starts each backend process on startup and tears them down on shutdown.

## Configuration

| Variable | Default | Purpose |
| --- | --- | --- |
| `MCP_GATEWAY_PORT` | `8001` | HTTP port bound as `http://0.0.0.0:<port>` |
| `MCP_GATEWAY_BASE_PATH` | `/sse` | Base path used by both GET (SSE) and POST (ingress) endpoints |
| `MCP_BACKEND_CODEX_PATH` | auto-discovered | Path to `Eleon.McpCodexGateway.Host.Stdio` |
| `MCP_BACKEND_CODEX_ARGS` | *(empty)* | Extra args for Codex stdio host |
| `MCP_BACKEND_SSH_PATH` | auto-discovered | Path to `Eleon.McpSshGateway.Host.Stdio` |
| `MCP_BACKEND_SSH_ARGS` | *(empty)* | Extra args for SSH stdio host |
| `ASPNETCORE_ENVIRONMENT` | `Development` | Standard ASP.NET Core environment flag |

## Run

```bash
cd C:/Workspace/src/server
dotnet run --project src/hosts/Eleon.McpGateway.Host.Sse
```

Optional pre-build: `dotnet build src/mcp.sln`.

## Quick checks

```bash
# Health
curl http://localhost:8001/health

# Initialize Codex backend (default)
curl -X POST http://localhost:8001/sse \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":"1","method":"initialize"}'

# Initialize SSH backend explicitly
curl -X POST http://localhost:8001/sse/ssh-stdio \
  -H "Content-Type: application/json" \
  -d '{"jsonrpc":"2.0","id":"1","method":"initialize"}'

# Watch Codex SSE stream
curl -N http://localhost:8001/sse

# Watch SSH SSE stream
curl -N http://localhost:8001/sse/ssh-stdio
```

The gateway keeps an independent SSE connection per backend, proxies client POSTs to the selected backend’s stdin, and streams all backend messages to the corresponding SSE subscriber.
