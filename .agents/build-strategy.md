# Eleonsoft AI Build + Diagnostics Performance Rules

**For large .NET solutions; fast, deterministic, cache-aware**

This document provides comprehensive build performance rules for AI agents operating on large .NET repositories. The primary objective is to **MINIMIZE wall-clock time, I/O, and CPU** while producing correct build truth and actionable diagnostics.

**Reference**: See `agents.md` section 12 (BUILD_PERF) for TIE-encoded summary.

## 0. CORE PRINCIPLE (NON-NEGOTIABLE)

**Agents MUST NOT execute expensive build commands repeatedly or parse full build output streams when cheaper, structured, or cached alternatives exist.**

Always choose the cheapest valid operation.

**Violation of these rules is a performance bug.** If you violate them, you MUST self-correct and explain the correction.

## 1. FORBIDDEN PATTERNS (HARD RULES)

You MUST NOT:

- **Run `dotnet build` repeatedly** to answer informational queries (e.g., "how many errors?").
- **Parse/pipe full build output** (stdout+stderr) through text filters (`Select-String`/`grep`) unless explicitly required and no structured alternative exists.
- **Trigger `dotnet restore`** unless dependency state changed or assets are missing/invalid.
- **Build the entire mega solution** when a smaller scope (`.csproj` / local `.sln` / `.slnf`) is sufficient.
- **Re-run builds** when an equivalent binlog exists for the same cache keys (commit/args/config/framework).
- **Run clean between Stage 1 and Stage 2** (it defeats incremental behavior).

### Examples of Violations

**BAD**: Running `dotnet build` three times to count errors:
```powershell
# WRONG - violates rule
$errors1 = (dotnet build 2>&1 | Select-String "error").Count
$errors2 = (dotnet build 2>&1 | Select-String "error").Count  # Violation!
$errors3 = (dotnet build 2>&1 | Select-String "error").Count  # Violation!
```

**GOOD**: Build once, analyze binlog:
```powershell
# CORRECT - build once, analyze binlog
dotnet build -bl:"build.binlog"
# Analyze binlog for error count (or reuse existing binlog if cache keys match)
```

**BAD**: Building entire solution when module solution exists:
```powershell
# WRONG - builds 50+ projects
dotnet build Eleonsoft.sln
```

**GOOD**: Build smallest relevant target:
```powershell
# CORRECT - builds only relevant module
dotnet build "src/eleonsoft/server/src/modules/Eleon.Billing.Module/Eleon.Billing.Module.sln"
```

## 2. REQUIRED BUILD PRACTICES

### 2.1 Structured Logging is Mandatory (binlog)

All agent-initiated builds MUST generate or reuse an MSBuild binary log (binlog).

`dotnet build` supports binary logging via `-bl`/`--binaryLogger`. The binlog can be opened in MSBuild Structured Log Viewer.

**Default binlog path (repo-relative):**
```
.agents/logs/build.{CACHEKEY}.binlog
```

**Where CACHEKEY is derived from:**
- git commit hash
- build target path (sln/slnf/csproj)
- Configuration (Debug/Release)
- Target framework (if specified)
- Any extra MSBuild properties/args

**Example:**
```powershell
$Commit = (git rev-parse HEAD).Trim()
$Target = "Eleon.Billing.Module.sln"
$Config = "Debug"
$KeyRaw = "$Commit|$Target|$Config"
$Key = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($KeyRaw))).Substring(0,16).ToLowerInvariant()
$Binlog = ".agents/logs/build.$Key.binlog"
dotnet build "$Target" -bl:"$Binlog"
```

### 2.1.1 BINLOG READ (TIE)

BINLOG_READ: binlog=build only (no test results); prefer MSBuild Structured Log Viewer; CLI read via MSBuild.StructuredLogger BinaryLog.ReadBuild; IF binlog exists: parse only (no rebuild).

BINLOG_CLI_EX:
```powershell
# temp reader (do NOT commit)
dotnet new console -n BinlogReader -o /tmp/binlog-reader
dotnet add /tmp/binlog-reader/BinlogReader.csproj package MSBuild.StructuredLogger
dotnet run --project /tmp/binlog-reader/BinlogReader.csproj -- ".agents/logs/build.<CACHEKEY>.binlog"
```

### 2.2 Restore Once, Then Never Restore Again in the Same Run

- If restore is needed, do it **ONCE** at the beginning.
- After that, ALL builds/tests must use `--no-restore`. `dotnet build` explicitly supports `--no-restore`.

**Example:**
```powershell
# Restore once (if needed)
dotnet restore "Eleon.Billing.Module.sln" -nologo

# All subsequent builds use --no-restore
dotnet build "Eleon.Billing.Module.sln" --no-restore -bl:"build.binlog"
dotnet test "Eleon.Billing.Module.sln" --no-restore
```

### 2.2.1 TEST TRX LOG (TIE)

TRX_USE: long/full test runs; CI parity; post-mortem diagnostics; durable test list.
TRX_CMD (with 5-minute timeout):
```powershell
$job = Start-Job -ScriptBlock { param($t) dotnet test $t -nologo -c <CFG> --no-restore --logger "trx;LogFileName=artifacts/test-results/<name>.trx" } -ArgumentList "<TARGET>"
$result = Wait-Job $job -Timeout 300
if ($result) { Receive-Job $job; Remove-Job $job } else { Stop-Job $job; Remove-Job $job; Write-Error "Test timeout after 5 minutes" }
```
TRX_READ: TRX=XML; quick scan `rg "<UnitTestResult outcome=\\\"Failed\\\""`; or parse XML (python3) to list failed tests; keep TRX in artifacts/test-results.
TRX_PY:
```powershell
python3 - <<'PY'
import sys,xml.etree.ElementTree as ET
p=sys.argv[1];r=ET.parse(p).getroot()
f=[e for e in r.iter() if e.tag.endswith('UnitTestResult') and e.get('outcome')=='Failed']
print("Failed",len(f));[print(e.get('testName')) for e in f[:20]]
PY artifacts/test-results/<name>.trx
```

### 2.3 Incremental and Scoped Builds Only

- Always attempt the **smallest relevant target first** (see Section 3).
- Do NOT clean between Stage 1 and Stage 2; MSBuild incremental builds skip targets that are up-to-date.

**Example:**
```powershell
# Stage 1: Full gate build (incremental)
dotnet build "Eleon.Billing.Module.sln" --no-restore -bl:"build.binlog"

# Stage 2: Errors-only scan (incremental, NO clean)
dotnet msbuild "Eleon.Billing.Module.sln" /m /nologo /v:m /clp:ErrorsOnly;Summary
```

### 2.4 Errors-Only Console Output is the Default for Scanning

When console output is required for diagnostics, prefer errors-only.

MSBuild supports console logger parameters including `ErrorsOnly` and `Summary` via `-clp`.

**Example:**
```powershell
# Errors-only output (fast, low noise)
dotnet msbuild "Eleon.Billing.Module.sln" /m /nologo /v:m /clp:ErrorsOnly;Summary
```

## 3. TARGET SELECTION ALGORITHM (SMALLEST-FIRST)

When asked to validate a change, triage failures, or "is the build broken?", pick the smallest target:

1. **Local solution near the change** (preferred):
   - `hosts/<X>/*.sln`, `modules/<Y>/*.sln`, `libs/<Z>/*.sln`
2. **Solution filter (*.slnf)** if present and relevant (preferred for large solutions):
   - `.slnf` is a JSON file selecting a subset of projects; MSBuild can build it directly starting with MSBuild 16.7.
3. **Mega solution only if needed**:
   - e.g., `Eleonsoft.Tests.sln` or full root solution(s)

**You MUST state which target you chose and why.**

### Examples

**Scenario**: User modified `Eleon.Billing.Module.Domain/Invoice.cs`

**BAD**: Building entire solution
```powershell
# WRONG - builds 50+ projects
dotnet build Eleonsoft.sln
```

**GOOD**: Building module solution
```powershell
# CORRECT - builds only Billing module
dotnet build "src/eleonsoft/server/src/modules/Eleon.Billing.Module/Eleon.Billing.Module.sln"
# Rationale: Change is in Billing module, module solution contains all related projects
```

**Scenario**: User modified shared library used by multiple modules

**GOOD**: Building solution filter or affected module solutions
```powershell
# Option 1: Solution filter (if available)
dotnet build "src/eleonsoft/server/src/shared/Eleon.Shared.Core.slnf"

# Option 2: Affected module solutions
dotnet build "src/eleonsoft/server/src/modules/Eleon.Billing.Module/Eleon.Billing.Module.sln"
dotnet build "src/eleonsoft/server/src/modules/Eleon.Orders.Module/Eleon.Orders.Module.sln"
```

## 4. STANDARD TWO-STAGE WORKFLOW (MANDATORY FOR ANY REAL BUILD)

Whenever you perform a build for truth, you MUST execute both stages:

### STAGE 1 — FULL GATE BUILD (truth: PASS/FAIL)

**Goal**: determine build health with minimal noise but real compilation.

**Command template (dotnet build):**
```powershell
dotnet build "<TARGET>" -nologo -maxcpucount --no-restore -v minimal -bl:"<BINLOG_PATH>"
```

**Notes:**
- `-maxcpucount` enables parallel build.
- `-v minimal` reduces noise while still showing errors.
- `-bl` writes the binlog for reuse.

**If Stage 1 requires restore** (assets missing), you may do:
```powershell
dotnet restore "<TARGET>" -nologo
# Then rerun Stage 1 with --no-restore
dotnet build "<TARGET>" -nologo -maxcpucount --no-restore -v minimal -bl:"<BINLOG_PATH>"
```

### STAGE 2 — FAST ERROR-ONLY SCAN (Top 10 + count)

**Goal**: extract the first actionable errors quickly with low noise, applying ignore rules.

**Preferred command (dotnet msbuild for logger control):**
```powershell
dotnet msbuild "<TARGET>" /m /nologo /v:m /clp:ErrorsOnly;Summary
```

MSBuild console logger parameters include `ErrorsOnly` and `Summary`.

**Then apply ignore filtering** using the repo file:
- `.agents/build-ignore.regex` (one regex per line, relative to repo root)

**Filtering rules:**
- Load ignore patterns; if file absent, treat as empty.
- Filter out any error lines matching ignore regex.
- Deduplicate identical lines.
- Output Top 10 remaining errors.
- Output total remaining error count after ignore filtering.

**IMPORTANT:**
- Stage 2 must be incremental; do not clean between stages.

### Complete Example

```powershell
# Setup
$RepoRoot = (Get-Location).Path
$AiDir = Join-Path $RepoRoot ".agents"
$LogDir = Join-Path $AiDir "logs"
New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

# Choose target (smallest-first)
$Target = "src/eleonsoft/server/src/modules/Eleon.Billing.Module/Eleon.Billing.Module.sln"

# Cache keys
$Commit = "unknown"
try { $Commit = (git rev-parse HEAD 2>$null).Trim() } catch { }
$Config = "Debug"
$KeyRaw = "$Commit|$Target|$Config"
$Key = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($KeyRaw))).Substring(0,16).ToLowerInvariant()
$Binlog = Join-Path $LogDir ("build." + $Key + ".binlog")

# Optional Restore (ONLY if required)
# dotnet restore "$Target" -nologo

# STAGE 1: Full Gate Build
dotnet build "$Target" -nologo -maxcpucount --no-restore -c $Config -v minimal -bl:"$Binlog"
$Stage1Exit = $LASTEXITCODE
"STAGE1_EXIT=$Stage1Exit"

# STAGE 2: Errors-only scan + ignore filtering + top10
$IgnoreFile = Join-Path $AiDir "build-ignore.regex"
$Ignore = @()
if (Test-Path $IgnoreFile) {
  $Ignore = Get-Content $IgnoreFile | Where-Object { $_ -and $_.Trim() -ne "" }
}
$IgnoreRegex = if ($Ignore.Count -gt 0) { "(" + ($Ignore -join "|") + ")" } else { $null }

# MSBuild errors-only output (fast)
$Lines = dotnet msbuild "$Target" /m /nologo /v:m /clp:ErrorsOnly;Summary 2>&1

# Keep only "error " lines, apply ignore regex if any
$Errors = $Lines | Where-Object {
  ($_ -match "error\s") -and (-not $IgnoreRegex -or ($_ -notmatch $IgnoreRegex))
}

$ErrorCount = ($Errors | Measure-Object).Count
"ERROR_COUNT=$ErrorCount"

# Top 10 unique
$Top10 = $Errors | Select-Object -Unique | Select-Object -First 10
"TOP_10_ERRORS:"
$Top10
```

## 5. BINLOG REUSE & QUERY RULES (BUILD ONCE, ANALYZE MANY)

If a valid binlog exists for the same cache keys, you MUST:

- **Reuse it** for error counting, diagnostics, reporting, summarization.
- **NOT rebuild** solely to answer: "How many errors?", "Which projects failed?", "What are the top errors?"

**Example:**

**BAD**: Rebuilding to count errors
```powershell
# WRONG - rebuilds just to count
$errors = (dotnet build 2>&1 | Select-String "error").Count
```

**GOOD**: Reusing existing binlog
```powershell
# CORRECT - reuse binlog if exists
$Binlog = ".agents/logs/build.$CacheKey.binlog"
if (Test-Path $Binlog) {
    # Analyze binlog for error count (use binlog viewer or structured analysis)
    # OR use Stage 2 errors-only scan (incremental, fast)
    dotnet msbuild "$Target" /m /nologo /v:m /clp:ErrorsOnly;Summary
} else {
    # Build once with binlog
    dotnet build "$Target" -bl:"$Binlog"
}
```

If root cause is unclear after Stage 2 output:

- Prefer inspecting the binlog (or ask for permission to use a binlog viewer/tooling).
- Only increase verbosity to diagnostic when a known failure needs deeper detail.

## 6. ERROR DETECTION SAFETY

You MUST NOT rely on naive generic string matching like `"error"` across massive logs.

**Allowed approaches:**
- Prefer structured binlog analysis.
- For Stage 2 errors-only scan, rely on MSBuild emitting only errors (`ErrorsOnly`) and keep lines intact.
- If you must match text, match `"error "` (error + trailing space) and preserve file/line/code context.

**Example:**

**BAD**: Naive string matching
```powershell
# WRONG - matches "error" anywhere, including in file paths, comments, etc.
$errors = $output | Select-String "error"
```

**GOOD**: Match "error " with trailing space
```powershell
# CORRECT - matches "error " (error + space), preserves context
$Errors = $Lines | Where-Object { $_ -match "error\s" }
```

## 7. REQUIRED OUTPUT FORMAT (EVERY RUN)

After you run the workflow, output ONLY the following sections (no megabyte logs):

### 1) Build Target Chosen
- `<TARGET PATH>` and why it was chosen (smallest-first rationale)

### 2) Cache Keys
- commit hash (or "unknown" if not available)
- configuration/framework (if used)
- full command line args that affect caching
- binlog path used

### 3) Stage 1 Result (Gate)
- PASS/FAIL
- exit code
- (no big logs)

### 4) Stage 2 Result (Errors-only)
- `ERROR_COUNT=<N>` after ignore filtering
- `TOP_10_ERRORS:`
  1) ...
  2) ...
  ...
- Keep original lines with file/line/error codes intact
- Dedupe identical lines

### 5) Next Action (single best step)
- One step only (e.g., "Fix compile error in Project X", "Switch to narrower module.sln", "Inspect binlog for target graph / restore issues")

### Example Output

```
Build Target Chosen: src/eleonsoft/server/src/modules/Eleon.Billing.Module/Eleon.Billing.Module.sln
Rationale: Change is in Billing module, module solution contains all related projects (smallest-first)

Cache Keys:
  commit: abc123def456...
  config: Debug
  framework: net9.0
  binlog: .agents/logs/build.1a2b3c4d5e6f7g8h.binlog

Stage 1 Result (Gate):
  PASS
  exit code: 0

Stage 2 Result (Errors-only):
  ERROR_COUNT=0

Next Action: Build successful, no errors found.
```

## 8. POWER SHELL REFERENCE IMPLEMENTATION (USE AS-IS)

```powershell
# Assumptions:
# - You are in repo root or a known workspace root.
# - .agents directory exists (create it if needed).
# - Ignore file is .\.agents\build-ignore.regex (one regex per line).

# --- Setup ---
$RepoRoot = (Get-Location).Path
$AiDir = Join-Path $RepoRoot ".agents"
$LogDir = Join-Path $AiDir "logs"
New-Item -ItemType Directory -Force -Path $AiDir | Out-Null
New-Item -ItemType Directory -Force -Path $LogDir | Out-Null

# Choose target by smallest-first (agent must set this):
$Target = "Eleonsoft.Tests.sln"  # example; prefer local *.sln or *.slnf if available

# Cache keys (best effort):
$Commit = "unknown"
try { $Commit = (git rev-parse HEAD 2>$null).Trim() } catch { }

$Config = "Debug"
$Framework = ""  # optional; set if used
$KeyRaw = "$Commit|$Target|$Config|$Framework"
$Key = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($KeyRaw))).Substring(0,16).ToLowerInvariant()
$Binlog = Join-Path $LogDir ("build." + $Key + ".binlog")

# --- Optional Restore (ONLY if required) ---
# dotnet restore "$Target" -nologo

# --- Stage 1: Full Gate Build ---
dotnet build "$Target" -nologo -maxcpucount --no-restore -c $Config -v minimal -bl:"$Binlog"
$Stage1Exit = $LASTEXITCODE

"STAGE1_EXIT=$Stage1Exit"

# --- Stage 2: Errors-only scan + ignore filtering + top10 ---
$IgnoreFile = Join-Path $AiDir "build-ignore.regex"
$Ignore = @()
if (Test-Path $IgnoreFile) {
  $Ignore = Get-Content $IgnoreFile | Where-Object { $_ -and $_.Trim() -ne "" }
}
$IgnoreRegex = if ($Ignore.Count -gt 0) { "(" + ($Ignore -join "|") + ")" } else { $null }

# MSBuild errors-only output (fast). ErrorsOnly + Summary are supported clp parameters.
$Lines = dotnet msbuild "$Target" /m /nologo /v:m /clp:ErrorsOnly;Summary 2>&1

# Keep only "error " lines, apply ignore regex if any
$Errors = $Lines | Where-Object {
  ($_ -match "error\s") -and (-not $IgnoreRegex -or ($_ -notmatch $IgnoreRegex))
}

$ErrorCount = ($Errors | Measure-Object).Count
"ERROR_COUNT=$ErrorCount"

# Top 10 unique
$Top10 = $Errors | Select-Object -Unique | Select-Object -First 10
"TOP_10_ERRORS:"
$Top10
```

### 8.1 TEST COMMANDS WITH TIMEOUT (MANDATORY)

**All PowerShell `dotnet test` commands MUST use a 5-minute timeout to prevent indefinite hangs.**

**BAD**: Commands without timeout (can hang indefinitely):
```powershell
# WRONG - no timeout, can hang
cd C:\Workspace\src\eleonsoft\server\src && dotnet test Eleonsoft.Tests.sln --verbosity normal --logger "console;verbosity=normal" 2>&1 | Select-Object -Last 100
```

**GOOD**: Commands with 5-minute timeout:
```powershell
# CORRECT - 5-minute timeout enforced
$TestTarget = "C:\Workspace\src\eleonsoft\server\src\Eleonsoft.Tests.sln"
$LogFile = ".agents/logs/test.$(Get-Date -Format 'yyyyMMddHHmmss').txt"
$job = Start-Job -ScriptBlock {
    param($target, $log)
    Set-Location (Split-Path $target -Parent)
    dotnet test (Split-Path $target -Leaf) --verbosity normal --logger "console;verbosity=normal" 2>&1 | Tee-Object $log
} -ArgumentList $TestTarget, $LogFile
$result = Wait-Job $job -Timeout 300
if ($result) {
    $output = Receive-Job $job
    Remove-Job $job
    $output | Select-Object -Last 100
    Get-Content $LogFile -Tail 100
} else {
    Stop-Job $job
    Remove-Job $job
    Write-Error "Test timeout after 5 minutes (300 seconds)"
    if (Test-Path $LogFile) { Get-Content $LogFile -Tail 100 }
}
```

**Alternative pattern (simpler, for console output only):**
```powershell
# CORRECT - 5-minute timeout with Job + Wait-Job
$job = Start-Job -ScriptBlock {
    param($target)
    cd (Split-Path $target -Parent)
    dotnet test (Split-Path $target -Leaf) --verbosity normal --logger "console;verbosity=normal" 2>&1
} -ArgumentList "C:\Workspace\src\eleonsoft\server\src\Eleonsoft.Tests.sln"
$result = Wait-Job $job -Timeout 300
if ($result) {
    Receive-Job $job | Select-Object -Last 100
    Remove-Job $job
} else {
    Stop-Job $job
    Remove-Job $job
    Write-Error "Test timeout after 5 minutes"
}
```

**Rule**: All `dotnet test` commands in PowerShell MUST use `Start-Job` + `Wait-Job -Timeout 300` (300 seconds = 5 minutes).

## 9. /ai DOCUMENTATION RESPONSIBILITY (REPO HYGIENE)

If you are asked to "add these rules to agents.md" or "bootstrap the repo", you MUST ensure these files exist and reflect this policy:

- **`agents.md`** (or core agents.md section):
  - Mandatory Build & Query Rules (Sections 0–8 summarized in TIE format)
- **`.agents/readme.md`**:
  - "AI Build & Diagnostic Strategy" (why the rules exist; performance rationale)
- **`.agents/build-strategy.md`** (this file):
  - Deep explanation: Build once/analyze many; why binlogs; why no piping; why scoped targets; why no clean between stages.
- **`.agents/build-ignore.regex`**:
  - One regex per line; include initial known-noise examples if provided by user.

The documentation must be explicit and enforceable (exact commands, not fuzzy advice).

## 10. PRE-FLIGHT SELF-CHECK (MUST RUN BEFORE ANY BUILD)

Before executing any build command, you MUST ask internally:

1. **Can this be answered from an existing binlog for the same cache keys?**
2. **Can this be answered without building at all?**
3. **Can I reduce scope** (local `.sln` / `.slnf` / `.csproj`) instead of mega solution?
4. **Is restore actually required**, or can I use `--no-restore`?

**If any answer reduces cost, you MUST choose that cheaper option.**

### Example Pre-Flight Check

**Scenario**: User asks "How many build errors are there?"

**BAD**: Build immediately
```powershell
# WRONG - builds without checking
$errors = (dotnet build 2>&1 | Select-String "error").Count
```

**GOOD**: Pre-flight check
```powershell
# CORRECT - check binlog first
$CacheKey = ComputeCacheKey(...)
$Binlog = ".agents/logs/build.$CacheKey.binlog"

if (Test-Path $Binlog) {
    # Answer from binlog (or Stage 2 incremental scan)
    dotnet msbuild "$Target" /m /nologo /v:m /clp:ErrorsOnly;Summary
} else {
    # Build once with binlog
    dotnet build "$Target" -bl:"$Binlog"
    # Then Stage 2 for errors
}
```

## 11. TEST FILTERING (MANUAL TESTS)

**Default behavior**: Tests marked with `[Trait("Category", "Manual")]` are excluded from test runs by default.

**Configuration**:
- `.runsettings` file in test project directory or solution root
- Environment variable: `RUN_MANUAL_TESTS=1` to include manual tests
- Test filter: `--filter "Category!=Manual"` (default) or `--filter "Category=Manual"` (manual only)

**Usage**:
```powershell
# Exclude manual tests (default)
dotnet test --filter "Category!=Manual"

# Include manual tests
$env:RUN_MANUAL_TESTS = "1"
dotnet test

# Or explicitly include
dotnet test --filter "Category=Manual"
```

**Manual tests** typically require:
- Network access (HttpClient tests)
- External services
- Special environment configuration
- Long execution times

## Summary

These rules ensure:
- **Performance**: Minimal wall-clock time, I/O, and CPU usage
- **Correctness**: Accurate build truth and diagnostics
- **Cache-awareness**: Reuse binlogs and incremental builds
- **Determinism**: Consistent, reproducible results
- **Test filtering**: Manual tests excluded by default to prevent CI/CD failures

**Always prefer the cheapest valid operation. Violations are performance bugs that must be self-corrected.**
