# Eleon MCP SSH Gateway (stdio)

Stdin/stdout JSON-RPC host that exposes the MCP SSH Gateway tools (`ssh.execute`, `ssh.listHosts`, `ssh.describeHost`). Designed for MCP clients that speak `stdio` (Claude Desktop, Cursor, Codex CLI, etc.).

## Quick start

```bash
cd C:/Workspace/src/server
dotnet run --project src/hosts/Eleon.McpSshGateway.Host.Stdio
```

If you prefer to build first, use `dotnet build src/mcp.sln` then point your MCP client at `src/hosts/Eleon.McpSshGateway.Host.Stdio/bin/<Configuration>/net9.0/Eleon.McpSshGateway.Host.Stdio.exe`.

## Configuration

- **Host catalog**: JSON file resolved in this order: `ELEON_MCP_HOSTS_PATH` (if set) ➜ nearest `.agents/logs/meta/mcp/hosts.json` walking upward from the executable ➜ `hosts.json` next to the binary. Missing files are tolerated but no hosts will be available.
- **Audit log**: `ELEON_MCP_AUDIT_PATH` overrides; otherwise the first reachable `.agents/logs/mcp-ssh-audit.log`, falling back to `mcp-ssh-audit.log` beside the binary.
- **Host entry fields** (per host):
  - `id`, `name`, `hostname`, `username`, optional `port` (default 22), `enabled` (default `true`), `tags` (array).
  - `allow` / `deny` glob lists. Deny is evaluated before allow; an empty allow list means **deny all**.
  - `auth` object with `mode` (`password` | `privateKey` | `agent`). Secrets can be inline (`password`, `privateKeyPassphrase`) or read from environment variables (`passwordEnv`, `privateKeyPassphraseEnv`). `privateKeyPath` is required for `privateKey` mode.
- **Command timeout**: `timeoutSeconds` request value is clamped to 1–300 seconds; default is 30 seconds.

Minimal catalog example (`.agents/logs/meta/mcp/hosts.json`):

```jsonc
{
  "hosts": [
    {
      "id": "lab",
      "name": "Local lab VM",
      "hostname": "127.0.0.1",
      "port": 2222,
      "username": "lab",
      "allow": [ "uptime", "ls *", "cat /var/log/*.log" ],
      "deny": [ "rm *", "sudo *" ],
      "auth": {
        "mode": "password",
        "passwordEnv": "LAB_SSH_PASSWORD"
      }
    }
  ]
}
```

## Operational notes

- Process stays attached to stdio; exit with Ctrl+C or by closing the client session.
- Every execution is appended to the audit log with stdout/stderr previews and duration.
- Agent mode requires `ssh`/`ssh-agent` available on `PATH` and `SSH_AUTH_SOCK` set.
- Keep real credentials out of source control—use the env-variable fields or agent mode instead.
