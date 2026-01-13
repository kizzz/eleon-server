# HealthChecks Migration Status

## Phase 1: Discovery & Assessment ✅ COMPLETE

### 1.1 Identify All Usage Points ✅
- [x] Found all `AddCommonHealthChecks` calls (3 locations)
- [x] Found all `AddEleonsoftHealthChecks` calls (2 locations)
- [x] Found all `UseEleonsoftHealthChecksMiddleware` calls (4 locations)
- [x] Documented current configuration sections
- [x] Identified EventBus and RabbitMQ checks (still use old interface)
- [x] Listed all affected projects/modules

**Files Identified:**
1. `EleoncoreHostModule.cs` - Uses `AddCommonHealthChecks`
2. `EleonHostModule.cs` - Uses `AddEleonsoftHealthChecks`
3. `EleonS3MigrationModule.cs` - Uses `AddCommonHealthChecks`
4. `EleonS3HttpApiHostModule.cs` - Uses `AddEleonsoftHealthChecks`
5. `EleonsoftHealthCheckExtensions.cs` - Wrapper method that calls `AddCommonHealthChecks`

### 1.2 Configuration Audit ⚠️ PENDING
- [ ] Extract current `HealthChecks` configuration from all projects
- [ ] Document which checks are enabled
- [ ] Note any custom configuration
- [ ] Verify SQL Server connection strings format
- [ ] Check for any custom health check implementations

**Note:** Configuration audit should be done per-project during deployment.

## Phase 2: Migration Execution ✅ COMPLETE

### 2.1 Update Service Registration ✅
- [x] `EleoncoreHostModule.cs` - Updated to V2 methods
- [x] `EleonHostModule.cs` - Updated to V2 methods
- [x] `EleonS3MigrationModule.cs` - Updated to V2 methods
- [x] `EleonS3HttpApiHostModule.cs` - Updated to V2 methods
- [x] `EleonsoftHealthCheckExtensions.cs` - Updated wrapper method
- [x] All old registrations commented for rollback reference
- [x] EventBus and RabbitMQ checks kept separate (coexist with V2)

**Migration Pattern Applied:**
```csharp
// Old
services.AddCommonHealthChecks(configuration);
// OR
services.AddEleonsoftHealthChecks(configuration);

// New
services.AddEleonHealthChecksCore(configuration);
services.AddHealthChecks()
    .AddEleonHealthChecksAll(configuration);
```

### 2.2 Update Endpoint Mapping ✅ COMPLETE (No Changes Needed)
- [x] All modules continue using `UseEleonsoftHealthChecksMiddleware()`
- [x] Middleware is backward compatible (uses new coordinator internally)
- [x] New endpoints available but not required during migration

**Decision:** Keep existing middleware for backward compatibility.

### 2.3 Update Configuration ⚠️ PENDING
- [ ] Add `EnableDiagnostics: false` to SQL Server section (if not present)
- [ ] Add `PublishOnFailure: true` to Publishing section
- [ ] Verify `RestartEnabled: false` in production
- [ ] Set `RestartRequiresAuth: true`

**Note:** Configuration updates should be done per-project during deployment.

## Phase 3: Testing & Verification ⚠️ PENDING

### 3.1 Unit Test Execution ⚠️ PENDING
**Prerequisites:**
- [ ] Restore NuGet packages
- [ ] Build all projects

**Commands:**
```bash
dotnet restore
dotnet test src/eleonsoft/server/src/shared/libs/Eleon.HealthChecks.Lib/tests/ --verbosity normal
```

### 3.2 Integration Test Execution ⚠️ PENDING
**Prerequisites:**
- [ ] Docker installed and running
- [ ] SQL Server container capability verified

**Commands:**
```bash
dotnet test src/eleonsoft/server/src/shared/libs/Eleon.HealthChecks.Lib/tests/HealthChecks.Integration.Tests/ --verbosity normal
```

### 3.3 Build Verification ⚠️ PENDING
**Commands:**
```bash
dotnet build src/eleonsoft/server/src/shared/libs/Eleon.HealthChecks.Lib/Eleon.HealthChecks.Lib.Full.csproj
dotnet build src/eleonsoft/server/src/shared/modules/Eleoncore.Host.Module/
dotnet build src/eleonsoft/server/src/shared/modules/Eleon.Host.Module/
dotnet build src/eleonsoft/server/src/modules/Eleon.S3.Module/
```

### 3.4 Runtime Verification ⚠️ PENDING
- [ ] Start application in development mode
- [ ] Verify health endpoints respond
- [ ] Verify health checks execute correctly
- [ ] Check logs for any errors
- [ ] Verify publishing works (if configured)
- [ ] Test UI page (if enabled)

## Phase 4: Staging Deployment ⚠️ PENDING

### 4.1 Pre-Deployment Checklist ⚠️ PENDING
- [ ] All tests passing
- [ ] All projects building
- [ ] Configuration reviewed
- [ ] Migration guide reviewed

### 4.2 Staging Deployment ⚠️ PENDING
- [ ] Deploy to staging environment
- [ ] Monitor application startup
- [ ] Verify health endpoints accessible
- [ ] Run manual health checks
- [ ] Monitor logs for errors

## Phase 5: Production Deployment ⚠️ PENDING

### 5.1 Production Pre-Deployment ⚠️ PENDING
- [ ] All staging tests passed
- [ ] No critical issues found
- [ ] Rollback plan documented
- [ ] Monitoring alerts configured

### 5.2 Production Deployment ⚠️ PENDING
- [ ] Deploy during low-traffic window
- [ ] Monitor application startup
- [ ] Verify health endpoints immediately
- [ ] Monitor for first 30 minutes

## Phase 6: Legacy Code Cleanup ⚠️ PENDING

### 6.1 Verification Period ⚠️ PENDING
**Wait period:** 2-4 weeks after production deployment

### 6.2 Legacy Code Removal ⚠️ PENDING
- [ ] Remove `HealthCheckManager.cs`
- [ ] Remove `IEleonsoftHealthCheck.cs`
- [ ] Remove `DefaultHealthCheck.cs`
- [ ] Remove old check implementations (if fully replaced)

## Summary

### ✅ Completed
- Phase 1.1: Discovery complete
- Phase 2.1: Service registration migration complete
- Phase 2.2: Endpoint mapping (kept middleware, backward compatible)

### ⚠️ Pending
- Phase 1.2: Configuration audit (per-project)
- Phase 2.3: Configuration updates (per-project)
- Phase 3: Testing & Verification
- Phase 4: Staging Deployment
- Phase 5: Production Deployment
- Phase 6: Legacy Code Cleanup

## Next Actions

1. **Restore and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Run Tests**
   ```bash
   dotnet test
   ```

3. **Configuration Review**
   - Review each project's `appsettings.json`
   - Add new configuration options as needed
   - Verify production settings

4. **Development Testing**
   - Start applications
   - Test health endpoints
   - Verify all checks work

5. **Staging Deployment**
   - Deploy to staging
   - Monitor and verify
   - Run integration tests

## Notes

- All code changes are backward compatible
- Old middleware still works
- EventBus/RabbitMQ checks can coexist
- Rollback is simple (uncomment old code)
- No breaking changes to public APIs
