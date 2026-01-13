InstructionsVersion: ae23262f-63f1-4787-a600-3cf04ff28e91

# BACKEND AGENT SPEC

**Reference**: For general workspace rules, multi-repo coordination, and frontend rules, see root [`agents.md`](../../../agents.md).

This document contains backend-specific (ABP, C#, .NET) rules and patterns for the server directory.

## 0. CTX

ROLE:
- C# backend (ABP/DDD/EF); modules/aggregates/services/controllers/tests; layering compliance

STACK:
- .NET / C# / ABP + DDD (services/modules)

PATHS:
- backend_root:
  - linux: /workspace/src/{eleonsoft|edi|immunities}/server/
  - win:   C:\Workspace\src\{eleonsoft|edi|immunities}\server

CTX (must consult when relevant):
- .agents/patterns/*.json
- .agents/architecture.json
- .agents/build-strategy.md
- root .agents/strategy.md (general strategy)
- root .agents/workspace.manifest.json

PD (consistency_over_cleverness):
- default: consistent, not clever
- if unsure: `rg` + codebase_search (semantic) → clone canonical → minimal delta
- ABP patterns: search existing modules first; NEVER invent new ABP architecture

## 1. BACKEND AGENT

BACKEND:
- role: C# backend (ABP/DDD/EF); modules/aggregates/services/controllers/tests; layering compliance
- tools:
  - deps intel: dotnet list <proj>.csproj package --outdated --include-transitive; Directory.Packages.props
- ctx: .agents/patterns/*.json; .agents/architecture.json

## 2. GLOBAL (Backend-Specific)

ALWAYS:
- plan 3–7 steps BEFORE any edits (group by repo + layer)
- keep change PR-sized (few files; minimal surface area)
- respect ABP deps (one-way):
  Domain.Shared → Infrastructure/EFCore → Domain → Application → Application.Contracts → HttpApi → HttpApi.Client → HttpApi.Host
- run narrowest build/tests/lint that proves change (scoped/affected)
- show concrete tool results (commands + output tail + exit codes)
- search existing patterns first (`rg` + codebase_search) → clone canonical → adapt
- verify after significant changes (MANDATORY after migrations/refactors)

NEVER:
- break ABP dependency direction
- reference Domain types from Application.Contracts (DTOs only)
- skip verify loop after migrations/refactors
- invent new patterns when an existing canonical pattern exists

UNSURE:
- prefer tools over guessing:
  - strategy: root .agents/strategy.md
  - workspace map: root .agents/workspace.manifest.json
  - playbooks: root .agents/AGENT_PLAYBOOKS.json
  - deps intel: dotnet list <proj>.csproj package --outdated --include-transitive OR Directory.Packages.props
- pattern workflow: `rg` + codebase_search (semantic) → clone canonical → minimal delta
- if architecture remains unclear: ask human (state what was found + options + safest default)

## 3. MCP (Backend-Specific)

QUICK:
- code search: `rg` (ripgrep command-line tool, MANDATORY for text search) | codebase_search (semantic)
- dependency intel: dotnet list <proj>.csproj package --outdated --include-transitive + inspect Directory.Packages.props
- csharp work: `rg` + codebase_search + read_file → minimal manual edits → verify
- docs: microsoft_docs (Microsoft stack) + context7 (3rd-party: ABP/etc)

RIPGREP PATTERNS (MANDATORY - use `rg` command):
- Find symbol definition:
  - Pattern: `\bMyTypeName\b` (use `-w` for word boundaries)
  - Scope: `*.cs` files under `server/src/**`
  - Example: `rg -w "TaskExecutionDomainService" -t cs "src/eleonsoft/server/src/" -C 2 -m 50`
  - Windows PowerShell: `rg -w "TaskExecutionDomainService" -t cs "src/eleonsoft/server/src/" -C 2 -m 50`
  - Use case: Finding class/interface/type definitions
- Find string literal usage:
  - Pattern: `"Permission.SystemLog.General"` (escape dots: `\.`)
  - Scope: `*.cs` files
  - Example: `rg "Permission\.SystemLog\.General" -t cs "src/eleonsoft/server/src/" -C 2 -m 50`
  - Windows PowerShell: `rg "Permission\.SystemLog\.General" -t cs "src/eleonsoft/server/src/" -C 2 -m 50`
  - Use case: Finding permission strings, constants, magic strings
- Find method calls:
  - Pattern: `\bMethodName\s*\(` (use `-w` for word boundaries)
  - Scope: `*.cs` files under `server/src/**`
  - Example: `rg -w "WithUnitOfWorkAsync" -t cs "src/eleonsoft/server/src/" -C 2 -m 50`
  - Windows PowerShell: `rg -w "WithUnitOfWorkAsync" -t cs "src/eleonsoft/server/src/" -C 2 -m 50`
- Find namespace imports:
  - Pattern: `^using\s+Namespace\.Name`
  - Scope: `*.cs` files
  - Example: `rg "^using\s+Eleon\." -t cs "src/eleonsoft/server/src/" -C 1 -m 50`
  - Windows PowerShell: `rg "^using\s+Eleon\." -t cs "src/eleonsoft/server/src/" -C 1 -m 50`
- Find with exclusions: `rg "pattern" -t cs -g "!**/bin/**" -g "!**/obj/**" "server/src/" -C 2`

DOCS (priority):
- microsoft_docs: C#/.NET/ASP.NET Core/EF/Azure/Identity (canonical on conflicts)
- context7: ABP/NATS/MassTransit (docs+code)
- rule: call docs MCPs; do not guess APIs; keep excerpts minimal

mcp.microsoft_docs:
- endpoint: https://learn.microsoft.com/api/mcp
- stable + real-time; query specific APIs/topics (not whole products)
- rule: call before other docs for Microsoft topics; canonical if conflicts

mcp.context7:
- stable 3rd-party docs+code
- flow: resolve-library-id (fuzzy) → pick best ID → get-library-docs <id> [topic/page]
- selection: exact name match first; higher coverage/snippets; reputation High/Medium; prefer top-ranked
- id format: /org/project OR /org/project/version

ABP_QUESTIONS: (`rg` + codebase_search) → mcp.context7 resolve-library-id abp → get-library-docs → mcp.microsoft_docs for .NET specifics → mcp.fetch for ABP docs URLs → search existing workspace modules; NEVER guess ABP patterns.

## 4. EXPLORATION (Backend-Specific)

C#: `rg` → codebase_search → read_file → manual edit. sequence: single-repo: `rg` + codebase_search → identify file/type → read_file → manual edit; 
understanding: `rg` → codebase_search → read_file → FindReferences (`rg`); modifying: FindReferences (`rg`) → manual edit → check errors/tests. 
backend-heavy → `rg` + codebase_search src/server.

## 5. SHELL (Backend-Specific)

dotnet_cli: build: dotnet build <path-to.csproj-or.sln> | test: dotnet test <path-to.sln-or-test-project>; rules: MUST follow section 12 BUILD_PERF (smallest-first target selection, two-stage workflow, binlog reuse, --no-restore after initial restore); use `dotnet list package --outdated` or inspect `Directory.Packages.props` for dependency info, test affected modules core logic, prefer smallest solution. nuke: targets: nuke Verify (local verification build+tests+checks) | nuke Ship (release pipeline); rule: use nuke targets when available instead of ad-hoc scripts.

## 6. ARCH

ABP: order: Domain.Shared → Infra/EFCore → Domain → Application → Application.Contracts → HttpApi → HttpApi.Client → HttpApi.Host; domain: aggregates/entities/value objects/domain svc/events only, no EF Core/HTTP/UI/logging infra deps; application: implements use cases, coordinates domain+external svc, depends Domain+Application.Contracts only, no direct EF/transports; application_contracts: CRITICAL DDD RULE: Application.Contracts MUST NEVER reference Domain; MUST use DTOs, never domain types; all interfaces return DTOs; application svc map domain → DTOs; decouples contracts from domain, preserves DDD boundaries; violation: Application.Contracts referencing Domain = architectural violation; domain_shared: shared constants, localization, DTOs, enums used across layers, no dependencies, foundation; infra_efcore: implements persistence/integrations DbContext, repos, adapters, depends Domain.Shared+Domain, EF types don't leak upwards; httpapi: defines controllers/endpoints, maps HTTP Application/DTOs, minimal logic (business rules Application/Domain), controllers return DTOs Application.Contracts, never domain types; httpapi_client: typed clients HttpApi, no business logic; host: composition root, wires modules, DI, migrations, cfg.
DDD: aggregates: enforce invariants aggregate root methods, avoid anemic domain models, keep behavior with data; domain_svc: encapsulate domain logic spanning aggregates; application_svc: coordinate domain operations+external svc, no persistence details, MUST map domain → DTOs before returning, never return domain entities/types directly from application svc interfaces; dto_separation: STRONG RULE: Application.Contracts defines DTOs, never references Domain; domain types internal implementation details, DTOs public contract; application svc perform domain → DTO mapping; preserves DDD boundaries, allows domain evolution without breaking contracts; events: domain events represent state changes, handled domain or app layers.

## 7. LANG

csharp_net: style: nullable enabled, avoid suppression unless logically proven; prefer async/await CancellationToken; use records/record structs value objects when appropriate; use pattern matching/switch expressions when clearer; di_cfg: prefer DI, avoid statics/singletons; use strongly-typed options where practical; testing: add/adjust tests non-trivial changes; prefer unit tests near domain logic (DomainTestBase, mocks), integration tests cross-service flows (ModuleTestBase<TStartupModule>, SQLite in-memory); test org: single-project (common) OR multi-project (complex modules); base classes: ModuleTestBase (integration), DomainTestBase (unit domain), ApplicationTestBase (unit app), CrossModuleTestBase (cross-module); data builders: fluent pattern {Module}TestDataBuilder; naming: {Class}Tests, {Method}_Should_{Behavior}(); solutions: Eleonsoft.sln (production only), Eleonsoft.Tests.sln (all tests); frameworks: xUnit, NSubstitute, FluentAssertions/Shouldly, Volo.Abp.TestBase, SQLite in-memory.

## 9. TEST ARCHITECTURE

TEST_ORG:
- layouts:
  - single-project (common): modules/{Module}/tests/{Module}.Test.csproj
  - multi-project (complex):
    - tests/{Module}.Domain.Tests/
    - tests/{Module}.Application.Tests/
    - tests/{Module}.EntityFrameworkCore.Tests/
    - tests/{Module}.TestBase/
- recommended folders (by intent):
  - tests/TestBase/            shared base classes + startup modules
  - tests/Domain/              domain units (entities, domain services)
  - tests/Application/         app units (app services, handlers)
  - tests/EntityFrameworkCore/ repo/integration against EF
  - tests/Integration/         workflows, cross-layer
  - tests/HttpApi/             controllers/hubs
  - tests/TestHelpers/         data builders, mocks, constants

BASE_CLASSES:
- ModuleTestBase<TStartupModule>:
  - integration tests; AbpIntegratedTest<TStartupModule>
  - SQLite in-memory; real EF + UOW helpers (WithUnitOfWorkAsync)
  - requires {Module}TestStartupModule
- DomainTestBase:
  - unit tests; no DB; mock repos/event bus/UOW/loggers (NSubstitute)
- ApplicationTestBase:
  - unit tests; mock domain services + current user/tenant/object mapper
- CrossModuleTestBase:
  - cross-module integration; CrossModuleTestStartupModule + MaximalHostModule
  - InMemory event bus + SQLite in-memory; UOW tx disabled

SOLUTIONS:
- Eleon.{Modulename}.{Host | Module | Lib}.sln: all test projects
  - includes *.Full.csproj, *.Test.csproj
- Eleonsoft.sln: production only (NO tests) (slow build)
- Eleonsoft.Tests.sln: all test projects {slow build and tests}
  - includes all *.Test.csproj


FRAMEWORKS:
- xUnit, NSubstitute, FluentAssertions or Shouldly
- Volo.Abp.TestBase, Volo.Abp.Autofac
- Volo.Abp.EntityFrameworkCore.Sqlite (SQLite in-memory)
- Microsoft.NET.Test.Sdk, xunit.runner.visualstudio, coverlet.collector

TEST_TYPES:
- unit: DomainTestBase / ApplicationTestBase (fast; mocks; no DB)
- integration: ModuleTestBase<TStartupModule> (SQLite in-memory; real EF/UOW)
- cross-module: CrossModuleTestBase (MaximalHost + InMemory bus + SQLite)

NAMING:
- class: {ClassUnderTest}Tests
- method: {MethodName}_Should_{ExpectedBehavior}()
- integration: {Feature}IntegrationTests / {Feature}WorkflowTests
- advanced: {ClassUnderTest}AdvancedTests

## 11. OUTPUT

ABP / .NET (required as applicable; include outputs):
- abp cli (if used): paste command + output tail + exit code
- dotnet test (scoped target) + summary + exit code
- EF sanity (when DbContext/migrations touched):
  - dotnet ef migrations list (or migration assembly check) + output tail + exit code
- DB migration apply (when applied; include outputs):
  - dotnet ef database update (or app migrator) + output tail + exit code

NOTE:
- choose "affected" scope whenever possible; avoid full repo runs unless required

## 12. BUILD_PERF

GOAL: minimize wall-clock/I-O/CPU; preserve correctness; produce diagnostics that humans+agents can act on.
RULE: never pay for the same build twice; rebuild w/o new inputs = perf bug → self-correct.

PREFLIGHT (before any build/test):
1) can answer w/o build/test? → SKIP + give next local cmd
2) reduce scope? (.csproj → .slnf → local .sln → mega .sln last)
3) restore needed? → default NO (use --no-restore)
4) output piping? avoid buffering traps (see SHELL_OUTPUT)

TARGET_SELECTION (smallest-first):
1) affected test project (*.Tests.csproj) closest to change
2) local .sln near change (module/area)
3) *.slnf (area filter)
4) mega .sln only if integration requires

FORBIDDEN:
- repeat dotnet build when binlog exists for same CACHEKEY
- dotnet test that triggers implicit build after Stage1 succeeded (always use --no-build)
- dotnet restore unless assets missing/inputs changed
- mega solution when smaller target proves change
- clean between Stage1/Stage2
- PowerShell `... | Select-Object -Last N` on long runs (buffers until end)

REQUIRED:
- binlog mandatory for Stage1 builds
- restore max once per target/config; afterwards ALWAYS --no-restore
- tests run with --no-build --no-restore after Stage1
- console logs: minimal/errors-first; durable logs saved to .agents/logs

SHELL_OUTPUT (PowerShell):
- NEVER: `dotnet ... 2>&1 | Select-Object -Last 100`  (buffers; appears "hung")
- NEVER: `dotnet test` without timeout (can hang indefinitely)
- DO:
  - stream + capture:
    - `dotnet <cmd> 2>&1 | Tee-Object <logfile>`
    - `Get-Content <logfile> -Tail 100`
  - test commands MUST use 5-minute timeout:
    - `$job = Start-Job -ScriptBlock { dotnet test <TARGET> ... }`
    - `$result = Wait-Job $job -Timeout 300` (300 seconds = 5 minutes)
    - `if ($result) { Receive-Job $job; Remove-Job $job } else { Stop-Job $job; Remove-Job $job; Write-Error "Test timeout after 5 minutes" }`

BINLOG/CACHE:
- BINLOG=.agents/logs/build.{CACHEKEY}.binlog
- CACHEKEY=SHA256(commit|target|config|framework|args)[0..15]
- if commit unknown: NO_GIT + stable fingerprint
- if BINLOG exists for CACHEKEY: ANALYZE ONLY; DO NOT re-run Stage1

BINLOG_READ (agent-friendly):
- binlog=build only (no test results)
- human: MSBuild Structured Log Viewer
- agent: prefer Stage2 ErrorsOnly output + Top hotspots summary; avoid binary parsing unless tooling exists

TWO_STAGE (no clean between):
STAGE0 (optional restore, once):
- dotnet restore "<TARGET>" -nologo

STAGE1 (gate + binlog):
- dotnet build "<TARGET>" -nologo -c <CFG> -m -v:minimal --no-restore -bl:"<BINLOG>"

STAGE2 (errors-only console + save):
- dotnet msbuild "<TARGET>" /m /nologo /v:m /clp:ErrorsOnly;Summary | Tee-Object ".agents/logs/build.errors.<CACHEKEY>.txt"
- apply .agents/build-ignore.regex (1 regex/line); dedupe (file,line,code,msg)
- output ERROR_COUNT + TOP_10

TESTING (after Stage1 succeeds; NO rebuild):
FAST (durable + CI-parity; MUST use 5-minute timeout in PowerShell; exclude Manual tests by default):
- PowerShell pattern (MANDATORY):
  ```powershell
  $job = Start-Job -ScriptBlock {
    param($t, $c, $k)
    dotnet test $t -nologo -c $c -m --no-restore --no-build --filter "Category!=Manual" --logger "trx;LogFileName=.agents/logs/test-results/tests.$k.trx"
  } -ArgumentList "<TARGET>", "<CFG>", "<CACHEKEY>"
  $result = Wait-Job $job -Timeout 300
  if ($result) { Receive-Job $job; Remove-Job $job } else { Stop-Job $job; Remove-Job $job; Write-Error "Test timeout after 5 minutes" }
  ```
- Linux/Bash: `timeout 300 dotnet test "<TARGET>" -nologo -c <CFG> -m --no-restore --no-build --filter "Category!=Manual" --logger "trx;LogFileName=.agents/logs/test-results/tests.<CACHEKEY>.trx"`
- Manual tests: excluded by default; set `RUN_MANUAL_TESTS=1` or use `--filter "Category=Manual"` to include

OPTIONAL (diagnose hangs/slow discovery; MUST use timeout):
- PowerShell: `$job = Start-Job -ScriptBlock { dotnet test "<TARGET>" --no-build --no-restore --filter "Category!=Manual" --blame }; Wait-Job $job -Timeout 300`
- PowerShell: `$job = Start-Job -ScriptBlock { dotnet test "<TARGET>" --no-build --no-restore --filter "Category!=Manual" --diag ".agents/logs/vstest.<CACHEKEY>.diag.txt" }; Wait-Job $job -Timeout 300`

TRX_READ (quick):
- rg "<UnitTestResult outcome=\"Failed\"" .agents/logs/test-results/tests.<CACHEKEY>.trx
- tiny py:
  python3 - <<'PY'
  import sys,xml.etree.ElementTree as ET
  p=sys.argv[1]; r=ET.parse(p).getroot()
  f=[e for e in r.iter() if e.tag.endswith('UnitTestResult') and e.get('outcome')=='Failed']
  print('Failed',len(f)); [print(e.get('testName')) for e in f[:20]]
  PY .agents/logs/test-results/tests.<CACHEKEY>.trx

PARALLELISM (use carefully; correctness first):
- build: always `-m`
- tests: prefer splitting by project (run multiple test csproj) over "one mega dotnet test"
- if vstest parallel is safe for the repo: append `-- --Parallel` (otherwise omit)

ERROR_DETECTION:
- avoid naive grep "error"
- prefer Stage2 ErrorsOnly + TRX failures
- if text match unavoidable: match "error " and keep file/line/code context

OUTPUT_FORMAT (always):
1) TARGET + why (scope justification)
2) CACHE (commit/config/framework/args) + BINLOG path
3) STAGE1 PASS/FAIL + exit code
4) STAGE2 ERROR_COUNT + TOP_10 (post-ignore) + link to saved .txt
5) TEST PASS/FAIL + exit code + TRX path
6) NEXT_ACTION (single best step)REF: .agents/build-strategy.md
