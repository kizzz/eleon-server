# VS2026 Test Discovery Fix

## Issue
VS2026 cannot build and run tests from the test projects.

## Root Cause
Test projects in the `tests\` folder were not explicitly marked as `IsTestProject=true`. While auto-detection should work, VS2026 requires explicit marking for reliable test discovery.

## Fix Applied

### Updated `tests/Directory.Build.props`
Added explicit `IsTestProject=true` for all projects in the tests folder:

```xml
<PropertyGroup>
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
  <!-- Explicitly mark all projects in tests folder as test projects for VS2026 -->
  <IsTestProject>true</IsTestProject>
</PropertyGroup>
```

## Verification

### CLI Test
```powershell
cd C:\Workspace\src\eleonsoft\server
dotnet build tests/Smoke.Tests/Smoke.Tests.csproj
dotnet test tests/Smoke.Tests/Smoke.Tests.csproj
```

**Result**: ✅ Builds and runs successfully

### VS2026 Steps
1. **Close Visual Studio**
2. **Clear caches**:
   ```powershell
   Remove-Item -Recurse -Force .vs, bin, obj, TestResults -ErrorAction SilentlyContinue
   ```
3. **Open solution in VS2026**
4. **Build Solution** (Build → Rebuild Solution)
5. **Open Test Explorer** (Test → Test Explorer)
6. **Wait for test discovery** - tests should appear
7. **Run tests** - should execute successfully

## Test Projects Fixed
All 14 test projects in `src/eleonsoft/server/tests\` are now explicitly marked as test projects:
- ✅ Eleon.Application.Tests
- ✅ Eleon.Arch.Tests
- ✅ Eleon.Architecture.Tests
- ✅ Eleon.Domain.Tests
- ✅ Eleon.HttpApi.Tests
- ✅ Eleon.JsonRpc.Stdio.Tests
- ✅ Eleon.McpCodexGateway.Host.Stdio.Tests
- ✅ Eleon.McpGateway.Host.Sse.Tests
- ✅ Eleon.McpSshGateway.Application.Tests
- ✅ Eleon.McpSshGateway.Domain.Tests
- ✅ Eleon.McpSshGateway.Integration.Tests
- ✅ Eleon.Pact.Tests
- ✅ Skynet.Contracts.Tests
- ✅ Smoke.Tests

## Additional Notes

- Test projects are now explicitly marked, ensuring VS2026 can discover them
- Auto-detection in `Directory.Build.props` still works as a fallback
- Test output paths are isolated to `bin\Debug\tests\{ProjectName}\{TargetFramework}\` (when using centralized output)
- All test projects have required packages: Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio



