# HealthChecks Library Security Documentation

## Security Posture

### Threat Model

**Attack Vectors:**
1. **Information Disclosure** - Health endpoints may leak sensitive data
2. **Denial of Service** - Expensive checks could be triggered repeatedly
3. **Unauthorized Actions** - Restart endpoint could be abused
4. **SQL Injection** - Custom SQL in health checks (mitigated by hardcoded queries)

### Attack Surface Analysis

**Public Endpoints:**
- `/health/live` - Minimal information, anonymous
- `/health/ready` - Readiness status, anonymous
- `/health/diag` - Full diagnostics, authenticated
- `/health/run` - Manual trigger, authenticated
- `/health/ui` - UI page, authenticated
- `/health/restart` - Service restart, authenticated, POST-only, off by default

**Risk Assessment:**
- **Low Risk**: `/health/live`, `/health/ready` (anonymous but minimal data)
- **Medium Risk**: `/health/diag`, `/health/run` (authenticated but sensitive data)
- **High Risk**: `/health/restart` (can cause service disruption)

---

## Endpoint Security

### `/health/live` - Liveness Probe

**Security:**
- Anonymous access allowed
- Returns minimal response: "OK" or HTTP 503
- No sensitive data exposed
- No internal state revealed

**Use Case:** Kubernetes liveness probes, load balancer health checks

**Configuration:**
```csharp
group.MapGet("/live", HandleLive)
    .WithName("HealthLive")
    .AllowAnonymous();
```

### `/health/ready` - Readiness Probe

**Security:**
- Anonymous access allowed
- Returns readiness status only
- No sensitive data exposed
- Filters to `ready` tag checks only

**Use Case:** Kubernetes readiness probes, dependency verification

**Configuration:**
```csharp
group.MapGet("/ready", HandleReady)
    .WithName("HealthReady")
    .AllowAnonymous();
```

### `/health/diag` - Diagnostics

**Security:**
- **Authentication Required**
- Returns full health check details
- Output scrubbing applied for non-privileged users
- Includes expensive diagnostic checks

**Use Case:** Troubleshooting, monitoring dashboards

**Configuration:**
```csharp
group.MapGet("/diag", HandleDiagnostics)
    .WithName("HealthDiagnostics")
    .RequireAuthorization();
```

**Output Scrubbing:**
- Non-privileged: Connection strings, tokens, stack traces scrubbed
- Privileged (Admin): Full details visible

### `/health/run` - Manual Trigger

**Security:**
- **Authentication Required**
- Triggers health check run
- Returns 202 Accepted (async)
- Returns 409 Conflict if already running

**Use Case:** On-demand health checks, testing

**Configuration:**
```csharp
group.MapPost("/run", HandleRun)
    .WithName("HealthRun")
    .RequireAuthorization();
```

### `/health/ui` - UI Page

**Security:**
- **Authentication Required**
- Full health check UI
- Output scrubbing based on user role

**Use Case:** Human-readable health dashboard

**Configuration:**
```csharp
group.MapGet("/ui", HandleUI)
    .WithName("HealthUI")
    .RequireAuthorization();
```

### `/health/restart` - Service Restart

**Security:**
- **Authentication Required**
- **POST-only** (GET rejected)
- **Off by default** (`RestartEnabled: false`)
- **Requires auth** (`RestartRequiresAuth: true` by default)
- Can cause service disruption

**Use Case:** Emergency restarts (use with extreme caution)

**Configuration:**
```json
{
  "HealthChecks": {
    "RestartEnabled": false,
    "RestartRequiresAuth": true
  }
}
```

**Middleware Check:**
```csharp
if (!_options.RestartEnabled)
{
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    return;
}

if (context.Request.Method != HttpMethods.Post)
{
    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
    return;
}

if (_options.RestartRequiresAuth && !context.User.Identity?.IsAuthenticated == true)
{
    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    return;
}
```

---

## SQL Server Safety Guarantees

### Hardcoded Safe Queries

**Readiness Check:**
```sql
SELECT
  1 AS ok,
  DB_NAME() AS db,
  @@SERVERNAME AS server_name,
  CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(50)) AS version;
```

**Safety Properties:**
- No `CREATE`, `ALTER`, `DROP` statements
- No `INSERT`, `UPDATE`, `DELETE` statements
- No `EXEC` or dynamic SQL
- Only reads metadata (system functions)
- Cannot create databases
- Cannot modify data

### No DB Creation

**Enforcement:**
1. **Query Allowlist** - Only hardcoded safe query allowed
2. **No Custom SQL** - Configuration validation rejects custom SQL
3. **Read-Only Intent** - Connection string can use `ApplicationIntent=ReadOnly`
4. **Permission-Based** - Login can have read-only permissions

**Proof:**
- Integration tests verify database count before/after
- Tests run under restricted login
- Contract tests verify query is hardcoded

### No Mutations

**Guarantees:**
- Readiness check never writes data
- Diagnostics check only reads system catalogs
- No business table reads in readiness
- No schema modifications

### Read-Only Enforcement

**Three Layers:**
1. **Principal Permissions** - Login/user with no write/DDL permissions
2. **Connection Intent** - `ApplicationIntent=ReadOnly` where applicable
3. **Query Policy** - Hardcoded allowlist, no custom SQL

---

## Output Scrubbing

### What Gets Scrubbed

**Patterns Detected:**
- Connection strings: Keys containing "connection", "connectionstring"
- Secrets: Keys containing "token", "password", "secret", "key"
- Stack traces: Keys containing "stacktrace", "exception"
- Values: Strings containing "data source", "server=", "password="

**Scrubbing Rules:**
```csharp
private static bool ShouldScrub(string key, string? value)
{
    var lowerKey = key.ToLowerInvariant();
    var lowerValue = value?.ToLowerInvariant() ?? "";

    // Connection strings
    if (lowerKey.Contains("connection") || lowerKey.Contains("connectionstring"))
        return true;

    // Secrets
    if (lowerKey.Contains("token") || lowerKey.Contains("password") || 
        lowerKey.Contains("secret") || lowerKey.Contains("key"))
        return true;

    // Stack traces
    if (lowerKey.Contains("stacktrace") || lowerKey.Contains("exception"))
        return true;

    // Value patterns
    if (lowerValue.Contains("data source") || lowerValue.Contains("server=") || 
        lowerValue.Contains("password="))
        return true;

    return false;
}
```

### Privileged vs Non-Privileged Mode

**Non-Privileged (Default):**
- All sensitive data scrubbed to `[REDACTED]`
- Safe summaries only
- No stack traces
- No connection strings

**Privileged (Admin):**
- Full details visible
- Stack traces included
- Connection strings visible
- Exception details complete

**Determination:**
```csharp
var isPrivileged = context.User?.IsInRole("Admin") ?? false;
var scrubbed = builder.ScrubSensitiveData(eto, isPrivileged);
```

### Configuration for Scrubbing Rules

Currently hardcoded. To extend:
1. Add scrubbing rules to `HealthReportBuilder`
2. Or create custom `IHealthReportBuilder` implementation
3. Configure via options if needed

---

## Best Practices

### Production Configuration

**Recommended Settings:**
```json
{
  "HealthChecks": {
    "Enabled": true,
    "RestartEnabled": false,
    "RestartRequiresAuth": true,
    "EnableDiagnostics": false,
    "PublishOnFailure": true,
    "PublishOnChange": false
  }
}
```

**Security Checklist:**
- [ ] `RestartEnabled: false` in production
- [ ] `RestartRequiresAuth: true`
- [ ] Authentication enabled for `/health/diag`, `/health/run`, `/health/ui`
- [ ] Output scrubbing enabled (default)
- [ ] Diagnostics disabled unless needed
- [ ] Network restrictions applied (see below)

### Network Restrictions

**Recommended:**
- Restrict `/health/diag`, `/health/run`, `/health/ui` to internal network
- Allow `/health/live`, `/health/ready` from load balancers
- Use firewall rules or reverse proxy restrictions
- Consider IP whitelisting for privileged endpoints

**Example (nginx):**
```nginx
# Allow live/ready from anywhere
location /health/live { }
location /health/ready { }

# Restrict diag/run/ui to internal network
location /health/diag {
    allow 10.0.0.0/8;
    deny all;
}
```

### Monitoring and Alerting

**What to Monitor:**
- Health check failures
- High latency checks
- Repeated failures (flapping)
- Publishing failures

**What to Alert On:**
- Critical check failures
- Publishing service down
- Unusual patterns (potential attacks)

**What NOT to Log:**
- Connection strings
- Passwords
- Tokens
- Full stack traces (unless privileged mode)

---

## Security Incident Response

### If Restart Endpoint Abused

1. **Immediate**: Disable `RestartEnabled` in configuration
2. **Investigate**: Check logs for unauthorized access
3. **Verify**: Ensure authentication is working
4. **Review**: Audit who has access to health endpoints

### If Sensitive Data Leaked

1. **Rotate**: Rotate all exposed credentials
2. **Review**: Check scrubbing rules
3. **Fix**: Update scrubbing patterns if needed
4. **Audit**: Review who accessed diagnostic endpoints

### If DoS Attack via Health Checks

1. **Throttle**: Reduce `MaxPublishesPerMinute`
2. **Timeout**: Reduce `CheckTimeout` for expensive checks
3. **Disable**: Temporarily disable expensive diagnostics
4. **Rate Limit**: Add rate limiting middleware

---

## Compliance Considerations

### Data Protection

- Health checks may contain system information
- Ensure compliance with data protection regulations
- Scrubbing helps but may not be sufficient for all requirements
- Consider additional encryption for sensitive environments

### Audit Logging

- Log all health check runs (type, initiator, status)
- Log all publishing attempts
- Log all restart attempts (if enabled)
- Retain logs per compliance requirements

---

## Security Testing

### Recommended Tests

1. **SQL Safety Tests** - Verify no DB creation
2. **Output Scrubbing Tests** - Verify sensitive data removed
3. **Authentication Tests** - Verify auth required for privileged endpoints
4. **Rate Limiting Tests** - Verify throttling works
5. **Input Validation Tests** - Verify malformed input handled

See `tests/HealthChecks.Contract.Tests/SqlServerSafetyInvariants.cs` for examples.
