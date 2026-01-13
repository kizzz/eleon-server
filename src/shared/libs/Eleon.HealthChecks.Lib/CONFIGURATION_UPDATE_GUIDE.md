# HealthChecks V2 Configuration Update Guide

## Overview

This guide helps you update existing `appsettings*.json` files to include the new V2 HealthChecks configuration options.

## Current Configuration Status

### Eleon.ServiceManager.Host
- **Location:** `src/eleonsoft/server/src/hosts/Eleon.ServiceManager.Host/`
- **Files:** `appsettings.app.json`, `appsettings.Release.json`, `appsettings.QA.json`, `appsettings.Debug.json`, `appsettings.ImmuRelease.json`
- **Status:** Has existing `HealthChecks` section with:
  - `ConfigurationCheck`
  - `HttpCheck`
  - `DiskSpaceCheck`
  - `RabbitMqCheck`

### Eleon.Eleoncore.Host
- **Location:** `src/eleonsoft/server/src/hosts/Eleon.Eleoncore.Host/`
- **Files:** `appsettings.app.json`, `appsettings.Release.json`, `appsettings.QA.json`, `appsettings.Debug.json`, `appsettings.ImmuRelease.json`
- **Status:** Has existing `HealthChecks` section with:
  - `ConfigurationCheck`
  - `HttpCheck`
  - `DatabaseMaxTablesSizeCheck`
  - `DiskSpaceCheck`

## Configuration Updates Required

### 1. Add V2 Core Options

Add these top-level options to the `HealthChecks` section:

```json
{
  "HealthChecks": {
    "Enabled": true,
    "ApplicationName": "<ApplicationName>",
    "CheckTimeout": 5,
    "EnableDiagnostics": false,
    "RestartEnabled": false,
    "RestartRequiresAuth": true,
    "PublishOnFailure": true,
    "PublishOnChange": false,
    "PublishIntervalMinutes": 5
  }
}
```

### 2. Add SqlServer Section

Add SQL Server configuration:

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

**Note:** `EnableDiagnostics: false` by default for safety. Only enable if needed.

### 3. Add Publishing Section

Add publishing configuration:

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

### 4. Update Existing Sections

Existing sections can remain as-is (they're backward compatible):
- `ConfigurationCheck` - No changes needed
- `HttpCheck` - No changes needed
- `DiskSpaceCheck` - Keep `Items[].Path` as `Logs` (resolved relative to the host output directory)
- `EnvironmentCheck` - Add if not present
- `CurrentProcessCheck` - Add if not present
- `SystemCheck` - Add if not present

## Per-Environment Configuration

### Development (`appsettings.Debug.json`, `appsettings.app.json`)
- `EnableDiagnostics: true` (optional, for debugging)
- `RestartEnabled: false` (recommended)
- `RestartRequiresAuth: true`

### Production (`appsettings.Release.json`, `appsettings.ImmuRelease.json`)
- `EnableDiagnostics: false` (required for safety)
- `RestartEnabled: false` (required)
- `RestartRequiresAuth: true` (required)

### QA/Staging (`appsettings.QA.json`)
- `EnableDiagnostics: false` (recommended)
- `RestartEnabled: false` (required)
- `RestartRequiresAuth: true`

## Example: Complete Configuration

See `CONFIGURATION_TEMPLATE.json` for a complete example.

## Migration Steps

1. **Backup existing config files**
2. **Add V2 core options** to each `appsettings*.json`
3. **Add SqlServer section** (if SQL checks are used)
4. **Add Publishing section** (if publishing is enabled)
5. **Verify production configs** have `RestartEnabled: false`
6. **Test in development** environment
7. **Deploy to staging** and verify
8. **Deploy to production** after verification

## Important Notes

- **Backward Compatible:** Existing configuration sections continue to work
- **Production Safety:** Always set `RestartEnabled: false` in production
- **SQL Safety:** Keep `EnableDiagnostics: false` unless specifically needed
- **Connection Strings:** Verify SQL Server connection strings are present and valid
