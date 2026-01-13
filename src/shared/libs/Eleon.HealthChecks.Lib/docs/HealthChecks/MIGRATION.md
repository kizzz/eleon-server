# HealthChecks Library Migration Guide

## Overview

This guide helps you migrate from the legacy HealthChecks architecture to the new redesigned version that uses Microsoft HealthChecks as the single execution engine.

### What Changed and Why

The redesign addresses critical correctness issues:
- **Broken timeout logic** - Fixed with proper CancellationToken handling
- **Thread safety** - Single-run enforcement with SemaphoreSlim
- **Lost results** - All Microsoft HealthChecks now integrated
- **MSSQL safety** - Hardcoded safe queries, no DB creation possible
- **Security** - Proper authentication and output scrubbing

### Breaking Changes Summary

1. **Interface Change**: `IEleonsoftHealthCheck` → `IHealthCheck`
2. **Manager Change**: `HealthCheckManager` → `IHealthRunCoordinator`
3. **Registration**: New extension methods in `HealthCheckExtensionsV2`
4. **Endpoints**: New minimal API endpoints (old middleware still works)

### Migration Timeline Recommendations

- **Phase 1** (Week 1): Update registration code, test in dev
- **Phase 2** (Week 2): Deploy to staging, verify all checks work
- **Phase 3** (Week 3): Deploy to production
- **Phase 4** (Week 4): Remove legacy code after full verification

---

## Step-by-Step Migration

### Step 1: Update NuGet Packages

No new packages required. The redesign uses existing Microsoft HealthChecks infrastructure.

### Step 2: Update Service Registration

#### Before (Old):
```csharp
services.AddCommonHealthChecks(configuration);
```

#### After (New):
```csharp
// Register core infrastructure
services.AddHealthChecks()
    .AddEleonHealthChecksCore(configuration)
    .AddEleonHealthChecksAll(configuration); // Registers all checks

// Or register individually:
services.AddHealthChecks()
    .AddEleonHealthChecksCore(configuration)
    .AddEleonHealthChecksSqlServer(configuration)
    .AddEleonHealthChecksHttp(configuration)
    .AddEleonHealthChecksEnvironment(configuration)
    .AddEleonHealthChecksConfiguration(configuration)
    .AddEleonHealthChecksSystem(configuration);
```

**Key Points:**
- `AddEleonHealthChecksCore` must be called first
- `AddEleonHealthChecksAll` is a convenience method that registers everything
- Individual methods allow selective registration

### Step 3: Update Endpoint Mapping

#### Option A: Use New Minimal API Endpoints (Recommended)
```csharp
app.MapHealthCheckEndpoints(); // Maps /health/live, /health/ready, /health/diag, etc.
```

#### Option B: Keep Existing Middleware (Backward Compatible)
```csharp
app.UseEleonsoftHealthChecksMiddleware(); // Still works, but uses new coordinator internally
```

### Step 4: Update Configuration

#### New Configuration Structure:
```json
{
  "HealthChecks": {
    "Enabled": true,
    "ApplicationName": "MyApp",
    "CheckTimeout": 5,
    "EnableDiagnostics": false,
    "PublishOnFailure": true,
    "PublishOnChange": false,
    "PublishIntervalMinutes": 5,
    "RestartEnabled": false,
    "RestartRequiresAuth": true,
    "SqlServer": {
      "EnableDiagnostics": false,
      "DiagnosticsCacheMinutes": 10,
      "MaxTablesInDiagnostics": 100
    },
    "Publishing": {
      "PublishOnFailure": true,
      "PublishOnChange": false,
      "PublishIntervalMinutes": 5,
      "MaxPublishesPerMinute": 1
    }
  }
}
```

### Step 5: Update Custom Checks (If Any)

#### Before:
```csharp
public class MyCustomCheck : DefaultHealthCheck
{
    public override string Name => "MyCheck";
    public override bool IsPublic => true;

    public override async Task ExecuteCheckAsync(HealthCheckReportEto report)
    {
        // Your logic
        report.Status = HealthCheckStatus.OK;
        report.Message = "OK";
    }
}
```

#### After:
```csharp
public class MyCustomCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Your logic here
            
            var data = new Dictionary<string, object>
            {
                ["mycheck.latency_ms"] = 100,
                ["mycheck.status"] = "OK"
            };
            
            return HealthCheckResult.Healthy("OK", data);
        }
        catch (OperationCanceledException)
        {
            return HealthCheckResult.Unhealthy("Cancelled");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Error: {ex.Message}");
        }
    }
}
```

**Registration:**
```csharp
services.AddHealthChecks()
    .AddCheck<MyCustomCheck>("my-custom", tags: new[] { "ready" });
```

### Step 6: Remove Legacy Code

After migration is complete and verified:

**Files to Delete:**
- `HealthChecks/General/HealthCheckManager.cs`
- `HealthChecks/General/IEleonsoftHealthCheck.cs`
- `HealthChecks/General/DefaultHealthCheck.cs`
- Old check implementations (if fully replaced)

**Update References:**
- Remove `AddCommonHealthChecks` calls
- Remove `IEleonsoftHealthCheck` implementations
- Update any direct `HealthCheckManager` usage

---

## Configuration Changes

### New Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `EnableDiagnostics` | bool | false | Enable expensive diagnostic checks |
| `PublishOnFailure` | bool | true | Publish when any check fails |
| `PublishOnChange` | bool | false | Publish on status change |
| `PublishIntervalMinutes` | int | 5 | Periodic publish interval |
| `RestartRequiresAuth` | bool | true | Require auth for restart endpoint |

### SQL Server Options

```json
{
  "HealthChecks": {
    "SqlServer": {
      "EnableDiagnostics": false,
      "DiagnosticsCacheMinutes": 10,
      "MaxTablesInDiagnostics": 100
    }
  }
}
```

### Publishing Options

```json
{
  "HealthChecks": {
    "Publishing": {
      "PublishOnFailure": true,
      "PublishOnChange": false,
      "PublishIntervalMinutes": 5,
      "MaxPublishesPerMinute": 1
    }
  }
}
```

---

## Breaking Changes

### 1. Interface Change: `IEleonsoftHealthCheck` → `IHealthCheck`

**Impact:** All custom checks must be rewritten

**Migration:** See Step 5 above

### 2. Manager Change: `HealthCheckManager` → `IHealthRunCoordinator`

**Impact:** Direct manager usage must be updated

**Before:**
```csharp
var manager = serviceProvider.GetRequiredService<HealthCheckManager>();
await manager.ExecuteHealthCheckAsync("manual", "user");
```

**After:**
```csharp
var coordinator = serviceProvider.GetRequiredService<IHealthRunCoordinator>();
await coordinator.RunAsync("manual", "user");
```

### 3. Endpoint Paths Changed

**Old:**
- `/api/health` - Health status
- `/healthchecks-ui` - UI page

**New:**
- `/health/live` - Liveness (anonymous)
- `/health/ready` - Readiness (anonymous)
- `/health/diag` - Diagnostics (authenticated)
- `/health/run` - Manual trigger (authenticated)
- `/health/ui` - UI page (authenticated)

**Note:** Old middleware paths still work for backward compatibility.

### 4. Response Format Changes

ETO format remains the same, but:
- Structured observations now available
- Data scrubbing applied for non-privileged users
- Status mapping: `Degraded` → `OK` (for backward compatibility)

---

## Backward Compatibility

### What Still Works

- Old middleware (`UseEleonsoftHealthChecksMiddleware`) - Uses new coordinator internally
- Old ETO format - Fully supported
- Old configuration sections - Automatically mapped
- Old check implementations - Can coexist during migration

### Deprecation Timeline

- **Month 1-2**: Both old and new systems work
- **Month 3**: Old system marked as deprecated
- **Month 4+**: Old system removed

### Using Both During Migration

You can use both registration methods during migration:

```csharp
// Old (for existing checks)
services.AddCommonHealthChecks(configuration);

// New (for new checks)
services.AddHealthChecks()
    .AddEleonHealthChecksCore(configuration)
    .AddEleonHealthChecksSqlServer(configuration);
```

The new coordinator will try to use new checks first, falling back to old manager if needed.

---

## Troubleshooting

### Issue: Checks not running

**Solution:** Ensure `AddEleonHealthChecksCore` is called before registering checks.

### Issue: Timeout errors

**Solution:** Increase `CheckTimeout` in configuration or check-specific timeouts.

### Issue: Diagnostics not working

**Solution:** Set `EnableDiagnostics: true` in `HealthChecks:SqlServer` section.

### Issue: Publishing not working

**Solution:** Verify `HealthPublishingService` is registered (automatic with `AddEleonHealthChecksCore`).

---

## Support

For issues or questions:
1. Check this migration guide
2. Review architecture documentation
3. Check test examples in `tests/` directory
4. Review `LEGACY_REMOVAL.md` for cleanup guidance
