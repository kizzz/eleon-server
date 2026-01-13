# HealthChecks Library Architecture

## Architecture Overview

The HealthChecks library is organized into four main layers:

```
┌─────────────────────────────────────────────────────────────┐
│                     API Layer (Endpoints)                     │
│  /health/live, /health/ready, /health/diag, /health/run      │
└───────────────────────┬───────────────────────────────────────┘
                        │
┌───────────────────────▼───────────────────────────────────────┐
│                    Delivery Layer (Publishers)                │
│  IHealthPublisher → HTTP/EventBus/Telegram publishers         │
└───────────────────────┬───────────────────────────────────────┘
                        │
┌───────────────────────▼───────────────────────────────────────┐
│                      Core Layer (Execution)                    │
│  IHealthRunCoordinator → HealthSnapshotStore → ReportBuilder  │
└───────────────────────┬───────────────────────────────────────┘
                        │
┌───────────────────────▼───────────────────────────────────────┐
│                    Checks Layer (IHealthCheck)                  │
│  SqlServer/Http/Disk/Config/Environment/Process checks        │
└────────────────────────────────────────────────────────────────┘
```

## Core Components

### IHealthRunCoordinator

**Purpose:** Coordinates health check runs with thread-safe single-run enforcement.

**Key Features:**
- Single-run-at-a-time using `SemaphoreSlim`
- Proper timeout enforcement with `CancellationToken`
- Tag filtering support
- Immutable snapshot creation

**Usage:**
```csharp
var coordinator = serviceProvider.GetRequiredService<IHealthRunCoordinator>();
var snapshot = await coordinator.RunAsync(
    type: "manual",
    initiatorName: "user",
    options: new HealthRunOptions { CheckTimeoutSeconds = 30 },
    ct: cancellationToken);
```

### IHealthSnapshotStore

**Purpose:** Stores and retrieves health check snapshots.

**Default Implementation:** `InMemoryHealthSnapshotStore`
- Thread-safe volatile reads
- Simple in-memory storage
- Can be extended to Redis/Postgres

**Usage:**
```csharp
var store = serviceProvider.GetRequiredService<IHealthSnapshotStore>();
store.Store(snapshot);
var latest = store.GetLatest();
```

### IHealthReportBuilder

**Purpose:** Converts Microsoft `HealthReport` to ETO models.

**Key Features:**
- Status mapping (Healthy → OK, Unhealthy → Failed)
- Data extraction to ExtraInformation
- Output scrubbing for sensitive data
- Observation extraction

**Usage:**
```csharp
var builder = serviceProvider.GetRequiredService<IHealthReportBuilder>();
var eto = builder.BuildHealthCheckEto(healthReport, id, type, initiator, createdAt);
var scrubbed = builder.ScrubSensitiveData(eto, isPrivileged: false);
```

### IHealthPublisher

**Purpose:** Publishes health check snapshots to external systems.

**Implementations:**
- `HttpHealthPublisher` - Wraps `IHealthCheckService`
- `EventBusHealthPublisher` - MassTransit (optional)
- `TelegramPublisher` - Telegram notifications (optional)

**Usage:**
```csharp
var publisher = serviceProvider.GetRequiredService<IHealthPublisher>();
await publisher.PublishAsync(snapshot, cancellationToken);
```

## Check Implementation Guide

### How to Implement IHealthCheck

**Basic Pattern:**
```csharp
public class MyHealthCheck : IHealthCheck
{
    private readonly ILogger<MyHealthCheck> _logger;

    public MyHealthCheck(ILogger<MyHealthCheck> logger)
    {
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Your check logic here
            var isHealthy = await PerformCheckAsync(cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                ["mycheck.latency_ms"] = 100,
                ["mycheck.status"] = isHealthy ? "OK" : "FAILED"
            };
            
            return isHealthy
                ? HealthCheckResult.Healthy("OK", data)
                : HealthCheckResult.Unhealthy("Failed", data: data);
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("Check cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }
}
```

### Structured Data Patterns

**Naming Convention:**
- `{checkname}.{metric}` - e.g., `sql.latency_ms`, `http.status_code`
- Use consistent prefixes per check type
- Include units in key names when applicable

**Data Types:**
- Numbers: `long` for milliseconds, `int` for counts
- Strings: Status messages, identifiers
- Booleans: Success/failure flags

**Example:**
```csharp
var data = new Dictionary<string, object>
{
    ["sql.latency_ms"] = 45L,
    ["sql.connections_total"] = 3,
    ["sql.connections_ok"] = 2,
    ["sql.connection_Db1.success"] = true,
    ["sql.connection_Db1.db"] = "MyDatabase"
};
```

### Tag Usage

**Standard Tags:**
- `live` - Liveness checks (minimal, fast)
- `ready` - Readiness checks (dependencies verified)
- `diag` - Diagnostic checks (expensive operations)

**Registration:**
```csharp
builder.AddCheck<MyCheck>("my-check", tags: new[] { "ready", "live" });
```

**Filtering:**
```csharp
var report = await healthCheckService.CheckHealthAsync(
    check => true, // Will filter by tags in registration
    cancellationToken);
```

### Cancellation Token Handling

**Always:**
1. Check `cancellationToken.IsCancellationRequested` before long operations
2. Pass token to all async I/O operations
3. Handle `OperationCanceledException` gracefully
4. Return appropriate status (usually `Unhealthy` with "Cancelled" message)

**Example:**
```csharp
cancellationToken.ThrowIfCancellationRequested();
await SomeAsyncOperation(cancellationToken);
```

## Security Model

### Endpoint Security Levels

| Endpoint | Authentication | Purpose | Data Scrubbing |
|----------|---------------|---------|----------------|
| `/health/live` | Anonymous | Liveness probe | Full |
| `/health/ready` | Anonymous | Readiness probe | Full |
| `/health/diag` | Required | Full diagnostics | Conditional |
| `/health/run` | Required | Manual trigger | N/A |
| `/health/ui` | Required | UI page | Conditional |
| `/health/restart` | Required, POST-only | Restart service | N/A |

### Output Scrubbing

**What Gets Scrubbed (non-privileged):**
- Connection strings
- Passwords, secrets, tokens
- API keys
- Stack traces
- Exception details (summary only)

**Privileged Mode:**
- Admin users see full details
- Non-admin users see scrubbed output
- Controlled by `isPrivileged` parameter

**Configuration:**
Scrubbing rules are hardcoded in `HealthReportBuilder.ScrubSensitiveData`. Extend for custom patterns.

### Privileged Mode

**Determination:**
```csharp
var isPrivileged = context.User?.IsInRole("Admin") ?? false;
var scrubbed = builder.ScrubSensitiveData(eto, isPrivileged);
```

**Behavior:**
- Privileged: Full exception details, stack traces, connection strings
- Non-privileged: Scrubbed values, safe summaries only

## Performance Considerations

### Caching Strategies

**Diagnostics Checks:**
- Use TTL caching for expensive operations
- Cache key per connection string
- Configurable cache duration (default 10 minutes)

**Example:**
```csharp
// In SqlServerDiagnosticsHealthCheck
if (_cache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > DateTime.UtcNow)
{
    return cached.Data; // Use cache
}
// Otherwise, run query and cache result
```

### Timeout Handling

**Per-Check Timeouts:**
- Configured in `HealthRunOptions.CheckTimeoutSeconds`
- Default: 30 seconds
- Enforced via `CancellationTokenSource.CancelAfter`

**Best Practices:**
- Keep readiness checks fast (< 5 seconds)
- Allow longer for diagnostics (30-60 seconds)
- Use cancellation tokens in all I/O operations

### Concurrent Execution Limits

**Coordinator:**
- Single-run-at-a-time enforced
- Concurrent requests return `null` (already running)
- Prevents resource exhaustion

**Publishers:**
- Throttling: Max publishes per minute
- Deduplication: Don't publish same snapshot twice
- Async: Publishers don't block coordinator

---

## Data Flow

### Health Check Run Flow

1. **Trigger**: Manual (`/health/run`) or scheduled (background service)
2. **Coordinator**: Acquires lock, creates cancellation token with timeout
3. **HealthCheckService**: Runs all registered `IHealthCheck` implementations
4. **Builder**: Converts `HealthReport` → `HealthCheckEto`
5. **Snapshot**: Creates immutable `HealthSnapshot`
6. **Store**: Stores snapshot (replaces previous)
7. **Publisher**: Publishes snapshot based on policies
8. **Response**: Returns snapshot or null (if already running)

### Publishing Flow

1. **Background Service**: `HealthPublishingService` runs periodically
2. **Policy Check**: Evaluates `PublishOnFailure`, `PublishOnChange`, interval
3. **Throttle Check**: Verifies not exceeding `MaxPublishesPerMinute`
4. **Publish**: Calls all registered `IHealthPublisher` implementations
5. **Deduplication**: Skips if same snapshot already published

---

## Extension Points

### Custom Publishers

```csharp
public class MyPublisher : IHealthPublisher
{
    public string Name => "MyPublisher";

    public async Task<bool> PublishAsync(HealthSnapshot snapshot, CancellationToken ct)
    {
        // Your publishing logic
        return true;
    }
}

// Registration
services.AddSingleton<IHealthPublisher, MyPublisher>();
```

### Custom Snapshot Store

```csharp
public class RedisHealthSnapshotStore : IHealthSnapshotStore
{
    // Implement Redis storage
}

// Registration
services.AddSingleton<IHealthSnapshotStore, RedisHealthSnapshotStore>();
```

### Custom Report Builder

```csharp
public class CustomReportBuilder : IHealthReportBuilder
{
    // Implement custom conversion logic
}

// Registration
services.AddSingleton<IHealthReportBuilder, CustomReportBuilder>();
```

---

## Best Practices

1. **Always honor CancellationToken** - Check and pass to all async operations
2. **Use structured data** - Consistent naming, include units
3. **Tag appropriately** - `live` for fast, `ready` for dependencies, `diag` for expensive
4. **Cache expensive operations** - Use TTL caching for diagnostics
5. **Handle exceptions** - Map to appropriate `HealthStatus`
6. **Log appropriately** - Warning for failures, Debug for normal operations
7. **Test thoroughly** - Unit tests for logic, integration tests for I/O
