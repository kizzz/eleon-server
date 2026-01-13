# McpGateway Module Test Runner Script
# This script runs tests excluding Manual tests by default
# Set $env:RUN_MANUAL_TESTS=1 to include manual tests

param(
    [switch]$IncludeManual,
    [string]$Filter = ""
)

$ErrorActionPreference = "Stop"

# Set environment variable if IncludeManual is specified
if ($IncludeManual) {
    $env:RUN_MANUAL_TESTS = "1"
    Write-Host "Including manual tests..." -ForegroundColor Yellow
} elseif (-not $env:RUN_MANUAL_TESTS) {
    # Default: exclude manual tests
    $testFilter = if ($Filter) { "$Filter&Category!=Manual" } else { "Category!=Manual" }
    Write-Host "Excluding manual tests by default (use -IncludeManual to include them)" -ForegroundColor Cyan
} else {
    $testFilter = $Filter
}

# Build the test filter
if (-not $testFilter) {
    if ($env:RUN_MANUAL_TESTS -eq "1" -or $env:RUN_MANUAL_TESTS -eq "true") {
        $testFilter = $Filter
    } else {
        $testFilter = if ($Filter) { "$Filter&Category!=Manual" } else { "Category!=Manual" }
    }
}

Write-Host "=== Running McpGateway Module Tests ===" -ForegroundColor Cyan
Write-Host ""

# Change to test directory
$testDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $testDir

try {
    # Restore packages
    Write-Host "Restoring packages..." -ForegroundColor Yellow
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Package restore failed!" -ForegroundColor Red
        exit 1
    }

    # Build test project
    Write-Host "Building test project..." -ForegroundColor Yellow
    dotnet build --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Test build failed!" -ForegroundColor Red
        exit 1
    }

    # Run tests with filter
    Write-Host "Running tests..." -ForegroundColor Yellow
    if ($testFilter) {
        Write-Host "Test filter: $testFilter" -ForegroundColor Gray
        $job = Start-Job -ScriptBlock {
            param($filter)
            dotnet test --no-build --verbosity normal --filter $filter 2>&1
        } -ArgumentList $testFilter
        $result = Wait-Job $job -Timeout 300
        if ($result) {
            $output = Receive-Job $job
            Remove-Job $job
            $output
            $exitCode = if ($output -match "Failed!\s+-\s+Failed:\s+(\d+)") { 1 } else { 0 }
            exit $exitCode
        } else {
            Stop-Job $job
            Remove-Job $job
            Write-Error "Test timeout after 5 minutes"
            exit 1
        }
    } else {
        $job = Start-Job -ScriptBlock {
            dotnet test --no-build --verbosity normal 2>&1
        }
        $result = Wait-Job $job -Timeout 300
        if ($result) {
            $output = Receive-Job $job
            Remove-Job $job
            $output
            $exitCode = if ($output -match "Failed!\s+-\s+Failed:\s+(\d+)") { 1 } else { 0 }
            exit $exitCode
        } else {
            Stop-Job $job
            Remove-Job $job
            Write-Error "Test timeout after 5 minutes"
            exit 1
        }
    }
} finally {
    Pop-Location
}
