# Legacy Code Removal Guide

## Files Marked for Removal

The following files are part of the old architecture and should be removed after migration:

### Core Legacy Files (DELETE after migration complete)

1. **`HealthChecks/General/HealthCheckManager.cs`**
   - Replaced by: `HealthChecks/Core/HealthRunCoordinator.cs`
   - Status: Still referenced in `HealthCheckStaticPageHelper` (backward compatible fallback)
   - Action: Remove after all consumers migrate to new coordinator

2. **`HealthChecks/General/IEleonsoftHealthCheck.cs`**
   - Replaced by: `Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck`
   - Status: Still used by old check implementations
   - Action: Remove after all checks converted to IHealthCheck

3. **`HealthChecks/General/DefaultHealthCheck.cs`**
   - Replaced by: Direct `IHealthCheck` implementation
   - Status: Still used by old check implementations
   - Action: Remove after all checks converted

### Old Check Implementations (DEPRECATE, keep for backward compatibility)

These old checks still implement `IEleonsoftHealthCheck` and extend `DefaultHealthCheck`:
- `HealthChecks/Checks/Database/DatabaseHealthCheck.cs` → Use `SqlServerReadinessHealthCheck`
- `HealthChecks/Checks/Database/DatabaseTablesSizeHealthCheck.cs` → Use `SqlServerDiagnosticsHealthCheck`
- `HealthChecks/Checks/Database/DatabaseMaxTablesSizeHealthCheck.cs` → Use `SqlServerDiagnosticsHealthCheck`
- `HealthChecks/Checks/Http/HttpHealthCheck.cs` → Use `HttpHealthCheckV2`
- `HealthChecks/Checks/Environment/EnvironmentHealthCheck.cs` → Use `EnvironmentHealthCheckV2`
- `HealthChecks/Checks/Environment/DiskSpaceHealthCheck.cs` → Use `DiskSpaceHealthCheckV2`
- `HealthChecks/Checks/Environment/CurrentProccessHealthCheck.cs` → Use `CurrentProcessHealthCheckV2`
- `HealthChecks/Checks/Configuration/ConfigurationHealthCheck.cs` → Use `ConfigurationHealthCheckV2`
- `HealthChecks/Checks/System/SystemLogHealthCheck.cs` → Use `SystemLogHealthCheckV2`

## Migration Checklist

- [ ] Update all registration code to use `HealthCheckExtensionsV2` methods
- [ ] Update all endpoints to use new `HealthCheckEndpoints` mapping
- [ ] Verify all checks are registered as `IHealthCheck` implementations
- [ ] Remove old `AddEleonHealthChecks` registration (keep only V2)
- [ ] Remove `HealthCheckManager` references
- [ ] Remove `IEleonsoftHealthCheck` implementations
- [ ] Remove `DefaultHealthCheck` base class
- [ ] Update `HealthCheckStaticPageHelper` to remove fallback to old manager
- [ ] Run full test suite to verify no regressions

## New Architecture Files

All new files follow the pattern:
- Core: `HealthChecks/Core/` - Execution, state, conversion
- Checks: `HealthChecks/Checks/{Type}/` - Health check implementations (IHealthCheck)
- Delivery: `HealthChecks/Delivery/` - Publishers
- API: `HealthChecks/Api/` - Endpoints
