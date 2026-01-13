#!/bin/bash
# HealthChecks Library Test Runner Script
# This script runs all tests for the HealthChecks library

set -e

echo "=== HealthChecks Library Test Suite ==="
echo ""

# Restore packages
echo "Restoring packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "Package restore failed!"
    exit 1
fi

# Build library
echo "Building HealthChecks library..."
dotnet build ../Eleon.HealthChecks.Lib.Full.csproj --no-restore
if [ $? -ne 0 ]; then
    echo "Library build failed!"
    exit 1
fi

# Build test projects
echo "Building test projects..."
dotnet build --no-restore
if [ $? -ne 0 ]; then
    echo "Test build failed!"
    exit 1
fi

echo ""
echo "=== Running Unit Tests ==="
echo ""

# Run Core tests
echo "Running Core tests..."
dotnet test HealthChecks.Core.Tests/HealthChecks.Core.Tests.csproj --no-build --verbosity normal
CORE_EXIT=$?

# Run Registration tests
echo "Running Registration tests..."
dotnet test HealthChecks.Registration.Tests/HealthChecks.Registration.Tests.csproj --no-build --verbosity normal
REG_EXIT=$?

# Run Checks tests
echo "Running Checks tests..."
dotnet test HealthChecks.Checks.Tests/HealthChecks.Checks.Tests.csproj --no-build --verbosity normal
CHECKS_EXIT=$?

# Run Delivery tests
echo "Running Delivery tests..."
dotnet test HealthChecks.Delivery.Tests/HealthChecks.Delivery.Tests.csproj --no-build --verbosity normal
DELIVERY_EXIT=$?

# Run API tests
echo "Running API tests..."
dotnet test HealthChecks.Api.Tests/HealthChecks.Api.Tests.csproj --no-build --verbosity normal
API_EXIT=$?

# Run Contract tests
echo "Running Contract tests..."
dotnet test HealthChecks.Contract.Tests/HealthChecks.Contract.Tests.csproj --no-build --verbosity normal
CONTRACT_EXIT=$?

echo ""
echo "=== Running Integration Tests ==="
echo "Note: Integration tests require Docker to be running"
echo ""

# Run Integration tests
echo "Running Integration tests..."
dotnet test HealthChecks.Integration.Tests/HealthChecks.Integration.Tests.csproj --no-build --verbosity normal
INTEGRATION_EXIT=$?

echo ""
echo "=== Test Summary ==="
echo "Core Tests: $CORE_EXIT"
echo "Registration Tests: $REG_EXIT"
echo "Checks Tests: $CHECKS_EXIT"
echo "Delivery Tests: $DELIVERY_EXIT"
echo "API Tests: $API_EXIT"
echo "Contract Tests: $CONTRACT_EXIT"
echo "Integration Tests: $INTEGRATION_EXIT"

TOTAL_EXIT=$((CORE_EXIT + REG_EXIT + CHECKS_EXIT + DELIVERY_EXIT + API_EXIT + CONTRACT_EXIT + INTEGRATION_EXIT))

if [ $TOTAL_EXIT -eq 0 ]; then
    echo ""
    echo "All tests passed!"
    exit 0
else
    echo ""
    echo "Some tests failed!"
    exit 1
fi
