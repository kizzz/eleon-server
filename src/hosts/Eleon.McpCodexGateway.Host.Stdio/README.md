# Eleon.McpCodexGateway.Host.Stdio

Stdin/stdout shim that runs `codex mcp-server` and pipes all JSON-RPC traffic between the parent process and the Codex CLI. Adds minimal logging, cancellation handling, and process-tree cleanup.

## Prerequisites

- `codex` CLI available on `PATH` (the host will log an error and exit if it cannot start it).
- .NET 9 runtime for building/running this host.

## How it works

1. Build the Codex command line from environment variables (defaults shown):
   - `CODEX_WORKSPACE_DIR` (default `/workspace`)
   - `CODEX_SANDBOX_MODE` (default `workspace-write`)
   - `CODEX_EXTRA_ARGS` (optional, space-delimited)
2. Launch `codex --cd <dir> --sandbox <mode> mcp-server [extra...]`.
3. Proxy stdin/stdout to the child process; prefix stderr with `[codex]`.
4. Propagate exit codes and kill the Codex process tree on Ctrl+C or parent shutdown.

## Run

```bash
cd C:/Workspace/src/server
dotnet run --project src/hosts/Eleon.McpCodexGateway.Host.Stdio
```

To tweak the Codex invocation:

```bash
set CODEX_WORKSPACE_DIR=C:/Workspace
set CODEX_SANDBOX_MODE=workspace-write
set CODEX_EXTRA_ARGS=--log-level debug
dotnet run --project src/hosts/Eleon.McpCodexGateway.Host.Stdio
```

You can connect MCP Inspector or any stdio-capable MCP client directly to the built binary under `src/hosts/Eleon.McpCodexGateway.Host.Stdio/bin/<Configuration>/net9.0/`.
