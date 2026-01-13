# McpGateway Module Tests

## Manual Tests Configuration

Tests marked with `[Trait("Category", "Manual")]` are **excluded by default** from test runs.

### Running Tests

**Exclude manual tests (default):**
```powershell
# Using the test runner script
.\run-tests.ps1

# Or using dotnet test directly
dotnet test --filter "Category!=Manual"
```

**Include manual tests:**
```powershell
# Using the test runner script
.\run-tests.ps1 -IncludeManual

# Or using environment variable
$env:RUN_MANUAL_TESTS = "1"
dotnet test

# Or using dotnet test directly
dotnet test --filter "Category=Manual"
```

### Manual Tests

The following test classes are marked as manual (use HttpClient):
- `McpStreamableControllerTests`
- `McpConcurrencyTests`
- `LegacySseCompatibilityTests`
- `McpStreamableControllerCorsTests`
- `McpStreamableControllerTolerantModeOffTests`
- `McpStreamableControllerTolerantModeOnTests`
- `SsePipelineTests`
- `McpStreamableControllerSseTests`
- `McpStreamableControllerNegativeTests`
- `HealthEndpointTests`
- `GatewayEndpointsTests`

These tests require network access or external services and should be run manually or in specific environments.
