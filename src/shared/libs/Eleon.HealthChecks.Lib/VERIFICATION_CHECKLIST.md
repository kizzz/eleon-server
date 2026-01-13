# HealthChecks Migration Verification Checklist

Use this checklist to verify the migration is complete and working correctly.

## Pre-Verification

- [ ] All NuGet packages restored (`dotnet restore`)
- [ ] All projects build successfully (`dotnet build`)
- [ ] No compilation errors
- [ ] No linter warnings

## Code Verification

### Service Registration
- [ ] `EleoncoreHostModule.cs` uses V2 methods
- [ ] `EleonHostModule.cs` uses V2 methods
- [ ] `EleonS3MigrationModule.cs` uses V2 methods
- [ ] `EleonS3HttpApiHostModule.cs` uses V2 methods
- [ ] `EleonsoftHealthCheckExtensions.cs` uses V2 methods
- [ ] Old registration code commented (for rollback reference)

### Build Verification
- [ ] HealthChecks library builds successfully
- [ ] All host modules build successfully
- [ ] All test projects build successfully
- [ ] No deprecated API warnings

## Test Verification

### Unit Tests
- [ ] Core tests pass (`HealthChecks.Core.Tests`)
- [ ] Registration tests pass (`HealthChecks.Registration.Tests`)
- [ ] Checks tests pass (`HealthChecks.Checks.Tests`)
- [ ] Delivery tests pass (`HealthChecks.Delivery.Tests`)
- [ ] API tests pass (`HealthChecks.Api.Tests`)
- [ ] Contract tests pass (`HealthChecks.Contract.Tests`)

### Integration Tests
- [ ] Docker is running
- [ ] SQL Server safety tests pass
- [ ] HTTP check integration tests pass
- [ ] All integration tests pass

**Commands:**
```bash
# Run all tests
cd tests
./run-tests.ps1  # Windows
./run-tests.sh   # Linux/Mac

# Or individually
dotnet test HealthChecks.Core.Tests/
dotnet test HealthChecks.Integration.Tests/
```

## Runtime Verification

### Development Environment

#### Application Startup
- [ ] Application starts without errors
- [ ] No exceptions in startup logs
- [ ] Health check services registered correctly
- [ ] Background services start correctly

#### Health Endpoints
- [ ] `GET /health/live` returns 200 OK
- [ ] `GET /health/ready` returns 200 OK (or 503 if unhealthy)
- [ ] `GET /health/diag` requires authentication
- [ ] `POST /health/run` requires authentication
- [ ] `GET /health/ui` requires authentication (if enabled)
- [ ] Old endpoints still work (backward compatibility)

#### Health Check Execution
- [ ] All registered checks execute
- [ ] SQL Server checks work (no DB creation)
- [ ] HTTP checks validate endpoints
- [ ] Environment checks report metrics
- [ ] Configuration checks validate settings
- [ ] System log checks work (if enabled)

#### Logging
- [ ] No errors in application logs
- [ ] Health check execution logged
- [ ] Publishing service logs (if enabled)
- [ ] No exceptions or warnings

#### Publishing (if enabled)
- [ ] Health check snapshots created
- [ ] Publishing service runs
- [ ] Snapshots published successfully
- [ ] No publishing errors

### Staging Environment

#### Pre-Deployment
- [ ] Configuration reviewed
- [ ] `RestartEnabled: false` confirmed
- [ ] `EnableDiagnostics: false` (unless needed)
- [ ] Authentication configured
- [ ] Connection strings valid

#### Deployment
- [ ] Application deploys successfully
- [ ] Application starts without errors
- [ ] Health endpoints accessible
- [ ] All checks execute successfully

#### Monitoring
- [ ] Application logs monitored
- [ ] Health check execution monitored
- [ ] Error rates monitored
- [ ] Response times acceptable
- [ ] No performance degradation

#### Verification Tests
- [ ] Normal operation - all checks pass
- [ ] Simulated failure - unhealthy status detected
- [ ] High load - no performance issues
- [ ] Concurrent requests - single-run enforcement works
- [ ] Timeout scenarios - proper cancellation
- [ ] Authentication - privileged endpoints protected

### Production Environment

#### Pre-Deployment
- [ ] All staging tests passed
- [ ] No critical issues found
- [ ] Rollback plan documented
- [ ] Monitoring alerts configured
- [ ] Team notified

#### Deployment
- [ ] Deployed during low-traffic window
- [ ] Application startup monitored
- [ ] Health endpoints verified immediately
- [ ] Initial health checks run
- [ ] First 30 minutes monitored

#### Post-Deployment (First 24 Hours)
- [ ] Health check execution monitored
- [ ] Error rates monitored
- [ ] Response times monitored
- [ ] Publishing success rate checked
- [ ] No database creation (SQL checks)
- [ ] System resources monitored

#### Post-Deployment (First Week)
- [ ] Daily health check review
- [ ] No regressions detected
- [ ] Performance metrics collected
- [ ] Issues documented
- [ ] Legacy code removal planned

## Configuration Verification

### Required Settings
- [ ] `HealthChecks:Enabled: true`
- [ ] `HealthChecks:ApplicationName` set correctly
- [ ] `HealthChecks:CheckTimeout` appropriate value
- [ ] `HealthChecks:RestartEnabled: false` (production)
- [ ] `HealthChecks:RestartRequiresAuth: true`
- [ ] `HealthChecks:SqlServer:EnableDiagnostics: false` (unless needed)

### Optional Settings
- [ ] `HealthChecks:PublishOnFailure: true` (if publishing enabled)
- [ ] `HealthChecks:PublishOnChange: false` (default)
- [ ] `HealthChecks:PublishIntervalMinutes: 5` (if publishing enabled)

### Connection Strings
- [ ] SQL Server connection strings valid
- [ ] Connection strings don't expose passwords in logs
- [ ] Multiple connection strings configured (if needed)

## Security Verification

### Endpoint Security
- [ ] `/health/live` - Anonymous access works
- [ ] `/health/ready` - Anonymous access works
- [ ] `/health/diag` - Requires authentication
- [ ] `/health/run` - Requires authentication
- [ ] `/health/ui` - Requires authentication
- [ ] `/health/restart` - Disabled or requires auth + POST

### Output Scrubbing
- [ ] Connection strings scrubbed for non-privileged users
- [ ] Passwords/tokens scrubbed
- [ ] Stack traces scrubbed (non-privileged)
- [ ] Privileged users see full details

### SQL Server Safety
- [ ] No database creation during checks
- [ ] No data mutations
- [ ] Only safe queries executed
- [ ] Read-only permissions sufficient

## Performance Verification

### Response Times
- [ ] `/health/live` responds in < 100ms
- [ ] `/health/ready` responds in < 500ms
- [ ] `/health/diag` responds in < 5s
- [ ] Health check execution completes within timeout

### Resource Usage
- [ ] No excessive CPU usage
- [ ] No excessive memory usage
- [ ] No excessive database connections
- [ ] No performance degradation

### Concurrency
- [ ] Single-run enforcement works
- [ ] Concurrent requests handled correctly
- [ ] No race conditions
- [ ] Thread-safe operations

## Rollback Verification

### Rollback Plan
- [ ] Old code still available (commented)
- [ ] Rollback procedure documented
- [ ] Rollback tested in staging
- [ ] Rollback ready if needed

### Backward Compatibility
- [ ] Old middleware still works
- [ ] Old ETO format supported
- [ ] Old configuration sections work
- [ ] EventBus/RabbitMQ checks coexist

## Documentation Verification

- [ ] Migration guide reviewed
- [ ] Architecture documentation reviewed
- [ ] Security documentation reviewed
- [ ] Team trained on new architecture
- [ ] Runbooks updated (if needed)

## Sign-Off

- [ ] All checks completed
- [ ] All tests passing
- [ ] All verifications successful
- [ ] Ready for production
- [ ] Team sign-off obtained

**Date:** _______________
**Verified by:** _______________
**Notes:** _______________
