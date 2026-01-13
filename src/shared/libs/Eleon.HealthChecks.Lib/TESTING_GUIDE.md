# HealthChecks Library Testing Guide

## Overview

This guide provides step-by-step instructions for testing the HealthChecks library after migration.

## Prerequisites

### Required Software
- [ ] .NET SDK 10.0 or later
- [ ] Docker Desktop (for integration tests)
- [ ] SQL Server (or Docker with SQL Server container)

### Required Access
- [ ] Access to development environment
- [ ] Access to staging environment (for deployment testing)
- [ ] Configuration files for each environment

## Step 1: Restore and Build

### Restore NuGet Packages

```bash
# From workspace root
cd src/eleonsoft/server/src/shared/libs/Eleon.HealthChecks.Lib
dotnet restore
```

**Expected:** All packages restored successfully

### Build Library

```bash
dotnet build Eleon.HealthChecks.Lib.Full.csproj
```

**Expected:** Build succeeds with no errors

### Build Test Projects

```bash
cd tests
dotnet restore
dotnet build
```

**Expected:** All test projects build successfully

## Step 2: Run Unit Tests

### Run All Unit Tests

```bash
# Windows
cd tests
.\run-tests.ps1

# Linux/Mac
cd tests
./run-tests.sh
```

### Run Individual Test Projects

**IMPORTANT**: All PowerShell `dotnet test` commands MUST use a 5-minute timeout to prevent indefinite hangs.

**PowerShell (with timeout):**
```powershell
# Helper function for test commands with timeout
function Run-TestWithTimeout {
    param([string]$TestProject, [string]$TestName)
    $job = Start-Job -ScriptBlock {
        param($proj)
        dotnet test $proj --verbosity normal 2>&1
    } -ArgumentList $TestProject
    $result = Wait-Job $job -Timeout 300
    if ($result) {
        Receive-Job $job
        Remove-Job $job
    } else {
        Stop-Job $job
        Remove-Job $job
        Write-Error "$TestName tests timed out after 5 minutes"
    }
}

# Core tests
Run-TestWithTimeout -TestProject "HealthChecks.Core.Tests/HealthChecks.Core.Tests.csproj" -TestName "Core"

# Registration tests
Run-TestWithTimeout -TestProject "HealthChecks.Registration.Tests/HealthChecks.Registration.Tests.csproj" -TestName "Registration"

# Checks tests
Run-TestWithTimeout -TestProject "HealthChecks.Checks.Tests/HealthChecks.Checks.Tests.csproj" -TestName "Checks"

# Delivery tests
Run-TestWithTimeout -TestProject "HealthChecks.Delivery.Tests/HealthChecks.Delivery.Tests.csproj" -TestName "Delivery"

# API tests
Run-TestWithTimeout -TestProject "HealthChecks.Api.Tests/HealthChecks.Api.Tests.csproj" -TestName "API"

# Contract tests
Run-TestWithTimeout -TestProject "HealthChecks.Contract.Tests/HealthChecks.Contract.Tests.csproj" -TestName "Contract"
```

**Linux/Bash (with timeout):**
```bash
# Core tests
timeout 300 dotnet test HealthChecks.Core.Tests/HealthChecks.Core.Tests.csproj --verbosity normal

# Registration tests
timeout 300 dotnet test HealthChecks.Registration.Tests/HealthChecks.Registration.Tests.csproj --verbosity normal

# Checks tests
timeout 300 dotnet test HealthChecks.Checks.Tests/HealthChecks.Checks.Tests.csproj --verbosity normal

# Delivery tests
timeout 300 dotnet test HealthChecks.Delivery.Tests/HealthChecks.Delivery.Tests.csproj --verbosity normal

# API tests
timeout 300 dotnet test HealthChecks.Api.Tests/HealthChecks.Api.Tests.csproj --verbosity normal

# Contract tests
timeout 300 dotnet test HealthChecks.Contract.Tests/HealthChecks.Contract.Tests.csproj --verbosity normal
```

**Expected Results:**
- All unit tests pass
- No test failures
- Test coverage report generated (if configured)

## Step 3: Run Integration Tests

### Prerequisites

1. **Start Docker Desktop**
   ```bash
   # Verify Docker is running
   docker ps
   ```

2. **Verify Testcontainers Support**
   - Docker Desktop must be running
   - Testcontainers will automatically pull SQL Server image

### Run Integration Tests

```bash
cd tests
dotnet test HealthChecks.Integration.Tests/HealthChecks.Integration.Tests.csproj --verbosity normal
```

**Expected Results:**
- SQL Server container starts automatically
- All integration tests pass
- Container cleanup after tests

**Key Tests:**
- `ReadinessCheck_ShouldNotCreateDatabase` - Verifies SQL safety
- `ReadinessCheck_ShouldNotMutateData` - Verifies no mutations
- `ReadinessCheck_ShouldReturnStructuredData` - Verifies data format
- `DiagnosticsCheck_ShouldCacheResults` - Verifies caching
- `HttpCheck_ShouldMeasureLatency` - Verifies HTTP checks

## Step 4: Build Host Modules

### Build Eleoncore Host Module

```bash
cd ../../modules/Eleoncore.Host.Module
dotnet restore
dotnet build
```

**Expected:** Build succeeds, no compilation errors

### Build Eleon Host Module

```bash
cd ../Eleon.Host.Module
dotnet restore
dotnet build
```

**Expected:** Build succeeds, no compilation errors

### Build S3 Modules

```bash
cd ../../../modules/Eleon.S3.Module
dotnet restore
dotnet build
```

**Expected:** Build succeeds, no compilation errors

## Step 5: Runtime Testing (Development)

### Start Application

```bash
# Start the application in development mode
dotnet run --project <host-project>
```

### Verify Startup

**Check Logs:**
- [ ] No errors during startup
- [ ] Health check services registered
- [ ] Background services started
- [ ] No exceptions

### Test Health Endpoints

#### Test Liveness Endpoint

```bash
# Should return 200 OK
curl http://localhost:5000/health/live
```

**Expected:** `OK` or `200 OK`

#### Test Readiness Endpoint

```bash
# Should return 200 OK if healthy, 503 if unhealthy
curl http://localhost:5000/health/ready
```

**Expected:** `200 OK` with JSON response or `503 Service Unavailable`

#### Test Diagnostics Endpoint (Requires Auth)

```bash
# Should require authentication
curl http://localhost:5000/health/diag
```

**Expected:** `401 Unauthorized` or full health data if authenticated

#### Test Manual Run Endpoint (Requires Auth)

```bash
# Should trigger health check
curl -X POST http://localhost:5000/health/run
```

**Expected:** `401 Unauthorized` or `202 Accepted` if authenticated

### Verify Health Check Execution

**Check Logs:**
- [ ] Health checks execute successfully
- [ ] SQL Server checks complete (no DB creation)
- [ ] HTTP checks validate endpoints
- [ ] Environment checks report metrics
- [ ] Configuration checks validate settings
- [ ] All checks report correct status

**Verify Snapshots:**
- [ ] Health check snapshots created
- [ ] Latest snapshot accessible
- [ ] Snapshots contain structured data

### Verify Publishing (if enabled)

**Check Logs:**
- [ ] Publishing service runs
- [ ] Snapshots published successfully
- [ ] No publishing errors
- [ ] Publishing respects policies (on failure, on change, interval)

## Step 6: Configuration Testing

### Test Configuration Options

**Enable Diagnostics:**
```json
{
  "HealthChecks": {
    "SqlServer": {
      "EnableDiagnostics": true
    }
  }
}
```

**Verify:**
- [ ] Diagnostics check registered
- [ ] Diagnostics check executes
- [ ] Results cached appropriately

**Disable Diagnostics:**
```json
{
  "HealthChecks": {
    "SqlServer": {
      "EnableDiagnostics": false
    }
  }
}
```

**Verify:**
- [ ] Diagnostics check not registered
- [ ] Only readiness check runs

### Test Publishing Options

**Publish on Failure:**
```json
{
  "HealthChecks": {
    "Publishing": {
      "PublishOnFailure": true
    }
  }
}
```

**Verify:**
- [ ] Publishing occurs when check fails
- [ ] Publishing doesn't occur when all checks pass

**Publish on Change:**
```json
{
  "HealthChecks": {
    "Publishing": {
      "PublishOnChange": true
    }
  }
}
```

**Verify:**
- [ ] Publishing occurs on status change
- [ ] Publishing doesn't occur when status unchanged

## Step 7: Security Testing

### Test Endpoint Security

**Anonymous Endpoints:**
- [ ] `/health/live` accessible without auth
- [ ] `/health/ready` accessible without auth

**Authenticated Endpoints:**
- [ ] `/health/diag` requires authentication
- [ ] `/health/run` requires authentication
- [ ] `/health/ui` requires authentication

**Restart Endpoint:**
- [ ] `/health/restart` disabled by default
- [ ] `/health/restart` requires POST method
- [ ] `/health/restart` requires authentication (if enabled)

### Test Output Scrubbing

**Non-Privileged User:**
- [ ] Connection strings scrubbed
- [ ] Passwords/tokens scrubbed
- [ ] Stack traces scrubbed

**Privileged User (Admin):**
- [ ] Full details visible
- [ ] Connection strings visible
- [ ] Stack traces visible

## Step 8: Performance Testing

### Test Response Times

```bash
# Measure response time
time curl http://localhost:5000/health/live
time curl http://localhost:5000/health/ready
```

**Expected:**
- `/health/live` < 100ms
- `/health/ready` < 500ms
- `/health/diag` < 5s

### Test Concurrent Requests

```bash
# Send 10 concurrent requests
for i in {1..10}; do
  curl http://localhost:5000/health/run &
done
wait
```

**Verify:**
- [ ] Only one health check runs at a time
- [ ] Other requests return 409 Conflict
- [ ] No race conditions
- [ ] Thread-safe execution

### Test Timeout Handling

**Configure Short Timeout:**
```json
{
  "HealthChecks": {
    "CheckTimeout": 1
  }
}
```

**Verify:**
- [ ] Health checks timeout correctly
- [ ] Timeout errors handled gracefully
- [ ] No hanging requests

## Step 9: SQL Server Safety Testing

### Verify No Database Creation

**Before Check:**
```sql
SELECT COUNT(*) FROM sys.databases;
```

**Run Health Check:**
```bash
curl http://localhost:5000/health/ready
```

**After Check:**
```sql
SELECT COUNT(*) FROM sys.databases;
```

**Expected:** Database count unchanged

### Verify No Data Mutations

**Before Check:**
```sql
SELECT COUNT(*) FROM TestTable;
```

**Run Health Check Multiple Times:**
```bash
for i in {1..5}; do
  curl http://localhost:5000/health/ready
done
```

**After Check:**
```sql
SELECT COUNT(*) FROM TestTable;
```

**Expected:** Row count unchanged

### Verify Read-Only Access

**Test with Read-Only Login:**
- [ ] Health check succeeds with read-only permissions
- [ ] No CREATE DATABASE permission required
- [ ] No write permissions required

## Step 10: Error Handling Testing

### Test Invalid Configuration

**Invalid Connection String:**
```json
{
  "ConnectionStrings": {
    "Default": "InvalidConnectionString"
  }
}
```

**Verify:**
- [ ] Health check returns Unhealthy
- [ ] Error message in data
- [ ] No application crash

### Test Network Failures

**Unreachable Endpoint:**
```json
{
  "HealthChecks": {
    "HttpCheck": {
      "Urls": [
        {
          "Name": "test",
          "Url": "https://unreachable.example.com"
        }
      ]
    }
  }
}
```

**Verify:**
- [ ] Health check returns Unhealthy or Degraded
- [ ] Error details in data
- [ ] No application crash

### Test Cancellation

**Long-Running Check:**
- [ ] Cancel request during execution
- [ ] Check cancels gracefully
- [ ] No hanging threads
- [ ] Proper cleanup

## Troubleshooting

### Build Errors

**Missing Packages:**
```bash
dotnet restore
dotnet clean
dotnet build
```

**Package Version Conflicts:**
- Check `Directory.Packages.props`
- Verify package versions
- Update if needed

### Test Failures

**Integration Tests Fail:**
- Verify Docker is running
- Check Docker logs
- Verify Testcontainers support

**Unit Tests Fail:**
- Check test output for details
- Verify mocks are set up correctly
- Check for missing dependencies

### Runtime Errors

**Service Registration Errors:**
- Verify `AddEleonHealthChecksCore` called first
- Verify `AddHealthChecks()` called before `AddEleonHealthChecksAll`
- Check service provider logs

**Health Check Execution Errors:**
- Check health check logs
- Verify configuration
- Check connection strings
- Verify permissions

## Test Results Template

```
=== HealthChecks Library Test Results ===

Date: _______________
Environment: _______________
Tester: _______________

Build Status: [ ] Pass [ ] Fail
Unit Tests: [ ] Pass [ ] Fail
Integration Tests: [ ] Pass [ ] Fail
Runtime Tests: [ ] Pass [ ] Fail
Security Tests: [ ] Pass [ ] Fail
Performance Tests: [ ] Pass [ ] Fail

Issues Found:
1. _______________
2. _______________

Notes:
_______________
```

## Next Steps After Testing

1. **Fix Any Issues** - Address test failures or errors
2. **Document Issues** - Record any problems found
3. **Update Configuration** - Adjust settings as needed
4. **Deploy to Staging** - Proceed with staging deployment
5. **Monitor** - Watch for any issues in staging
