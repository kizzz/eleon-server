# HealthChecks Library Test Runner Script
# This script runs all tests for the HealthChecks library

Write-Host "=== HealthChecks Library Test Suite ===" -ForegroundColor Cyan
Write-Host ""

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Package restore failed!" -ForegroundColor Red
    exit 1
}

# Build library
Write-Host "Building HealthChecks library..." -ForegroundColor Yellow
dotnet build ..\Eleon.HealthChecks.Lib.Full.csproj --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Library build failed!" -ForegroundColor Red
    exit 1
}

# Build test projects
Write-Host "Building test projects..." -ForegroundColor Yellow
dotnet build --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Test build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Running Unit Tests ===" -ForegroundColor Cyan
Write-Host ""

# Helper function to run tests with 5-minute timeout
function Run-TestWithTimeout {
    param(
        [string]$TestProject,
        [string]$TestName
    )
    Write-Host "Running $TestName tests..." -ForegroundColor Yellow
    $job = Start-Job -ScriptBlock {
        param($proj)
        dotnet test $proj --no-build --verbosity normal 2>&1
    } -ArgumentList $TestProject
    $result = Wait-Job $job -Timeout 300
    if ($result) {
        $output = Receive-Job $job
        Remove-Job $job
        $output
        return 0
    } else {
        Stop-Job $job
        Remove-Job $job
        Write-Host "$TestName tests timed out after 5 minutes!" -ForegroundColor Red
        return 1
    }
}

# Run Core tests
$coreExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Core.Tests\HealthChecks.Core.Tests.csproj" -TestName "Core"

# Run Registration tests
$regExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Registration.Tests\HealthChecks.Registration.Tests.csproj" -TestName "Registration"

# Run Checks tests
$checksExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Checks.Tests\HealthChecks.Checks.Tests.csproj" -TestName "Checks"

# Run Delivery tests
$deliveryExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Delivery.Tests\HealthChecks.Delivery.Tests.csproj" -TestName "Delivery"

# Run API tests
$apiExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Api.Tests\HealthChecks.Api.Tests.csproj" -TestName "API"

# Run Contract tests
$contractExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Contract.Tests\HealthChecks.Contract.Tests.csproj" -TestName "Contract"

Write-Host ""
Write-Host "=== Running Integration Tests ===" -ForegroundColor Cyan
Write-Host "Note: Integration tests require Docker to be running" -ForegroundColor Yellow
Write-Host ""

# Run Integration tests
$integrationExitCode = Run-TestWithTimeout -TestProject "HealthChecks.Integration.Tests\HealthChecks.Integration.Tests.csproj" -TestName "Integration"

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Cyan
Write-Host "Core Tests: $coreExitCode" -ForegroundColor $(if ($coreExitCode -eq 0) { "Green" } else { "Red" })
Write-Host "Registration Tests: $regExitCode" -ForegroundColor $(if ($regExitCode -eq 0) { "Green" } else { "Red" })
Write-Host "Checks Tests: $checksExitCode" -ForegroundColor $(if ($checksExitCode -eq 0) { "Green" } else { "Red" })
Write-Host "Delivery Tests: $deliveryExitCode" -ForegroundColor $(if ($deliveryExitCode -eq 0) { "Green" } else { "Red" })
Write-Host "API Tests: $apiExitCode" -ForegroundColor $(if ($apiExitCode -eq 0) { "Green" } else { "Red" })
Write-Host "Contract Tests: $contractExitCode" -ForegroundColor $(if ($contractExitCode -eq 0) { "Green" } else { "Red" })
Write-Host "Integration Tests: $integrationExitCode" -ForegroundColor $(if ($integrationExitCode -eq 0) { "Green" } else { "Red" })

$totalExitCode = $coreExitCode + $regExitCode + $checksExitCode + $deliveryExitCode + $apiExitCode + $contractExitCode + $integrationExitCode

if ($totalExitCode -eq 0) {
    Write-Host ""
    Write-Host "All tests passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host ""
    Write-Host "Some tests failed!" -ForegroundColor Red
    exit 1
}
