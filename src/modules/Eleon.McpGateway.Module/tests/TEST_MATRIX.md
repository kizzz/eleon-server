# MCP Streamable HTTP Phase 1 Test Matrix

## Overview

This document provides a comprehensive mapping of test scenarios to test files, ensuring full coverage of Phase 1 MCP Streamable HTTP functionality.

## Test Execution Targets

- **Total execution time**: < 30 seconds
- **Test framework**: xUnit
- **Assertion library**: FluentAssertions
- **Test infrastructure**: WebApplicationFactory for integration tests

## Test File → Scenario Mapping

### 1. Session Lifecycle Tests

**File**: `test/Infrastructure/Sessions/McpSessionRegistryTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Create new session when sessionId is null | `GetOrCreateAsync_CreatesNewSession_WhenSessionIdIsNull` | ✅ |
| Return existing session when sessionId provided | `GetOrCreateAsync_ReturnsExistingSession_WhenSessionIdProvided` | ✅ |
| Create different sessions for different IDs | `GetOrCreateAsync_CreatesDifferentSessions_ForDifferentIds` | ✅ |
| Throw exception on backend name mismatch | `GetOrCreateAsync_Throws_WhenBackendNameMismatch` | ✅ |
| Update LastAccessedAt on Touch | `TouchAsync_UpdatesLastAccessedAt` | ✅ |
| Throw exception when touching non-existent session | `TouchAsync_Throws_WhenSessionNotFound` | ✅ |
| Dispose backend and remove session on Terminate | `TerminateAsync_DisposesBackend_AndRemovesSession` | ✅ |
| Terminate is idempotent | `TerminateAsync_IsIdempotent` | ✅ |
| Thread-safe session creation | `GetOrCreateAsync_IsThreadSafe` | ✅ |
| Concurrent creation with same ID returns same session | `GetOrCreateAsync_ConcurrentCreation_SameSessionId_ReturnsSameSession` | ✅ |
| Concurrent creation with different IDs | `GetOrCreateAsync_ConcurrentCreation_DifferentIds_CreatesDifferentSessions` | ✅ |
| Respect cancellation token in GetOrCreate | `GetOrCreateAsync_RespectsCancellationToken` | ✅ |
| Respect cancellation token in Terminate | `TerminateAsync_RespectsCancellationToken` | ✅ |
| TryGetAsync returns null for non-existent session | `TryGetAsync_NonExistentSession_ReturnsNull` | ✅ |
| Sessions are isolated and don't interfere | `Sessions_AreIsolated_DoNotInterfere` | ✅ |

**File**: `test/HttpApi/McpStreamableControllerTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Initialize creates session and returns session ID | `Initialize_WithoutSessionHeader_CreatesSession_ReturnsSessionId` | ✅ |
| Initialize with existing session ID uses existing session | `Initialize_WithSessionHeader_UsesExistingSession` | ✅ |
| POST without session header returns 400 when not tolerant | `Post_WithoutSessionHeader_Returns400_WhenNotTolerant` | ✅ |
| Request with ID returns matching response | `Post_WithRequestId_ReturnsMatchingResponse` | ✅ |
| Notification without ID returns 202 | `Post_Notification_WithoutId_Returns202` | ✅ |
| GET without session header returns 400 | `Get_WithoutSessionHeader_Returns400` | ✅ |
| GET with session header establishes SSE and receives messages | `Get_WithSessionHeader_EstablishesSse_ReceivesMessages` | ✅ |
| SSE sends keepalive comments | `Get_Sse_SendsKeepaliveComments` | ✅ |
| SSE formats events correctly | `Get_Sse_FormatsEventsCorrectly` | ✅ |
| DELETE without session header returns 400 | `Delete_WithoutSessionHeader_Returns400` | ✅ |
| DELETE with session header terminates session | `Delete_WithSessionHeader_TerminatesSession_Returns204` | ✅ |
| CORS exposes Mcp-Session-Id header | `Cors_ExposesMcpSessionIdHeader` | ✅ |
| Request timeout returns error | `Post_WithRequestId_HandlesTimeout_ReturnsTimeoutError` | ✅ |
| Multiple sessions can coexist | `MultipleSessions_CanCoexist` | ✅ |
| Session cannot be used after termination | `Session_CannotBeUsedAfterTermination` | ✅ |
| Session ID header is case-insensitive | `SessionId_HeaderIsCaseInsensitive` | ✅ |
| Multiple concurrent requests with different IDs return correct responses | `MultipleConcurrentRequests_DifferentIds_ReturnCorrectResponses` | ✅ |
| Response correlation works across multiple sessions | `ResponseCorrelation_WorksAcrossMultipleSessions` | ✅ |

### 2. SSE Behavior Tests

**File**: `test/HttpApi/McpStreamableControllerSseTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| SSE connection handles client disconnect gracefully | `Sse_ConnectionHandlesClientDisconnect_Gracefully` | ✅ |
| SSE connection handles server cancellation | `Sse_ConnectionHandlesServerCancellation_Gracefully` | ✅ |
| SSE sends multiple messages in sequence | `Sse_SendsMultipleMessages_InSequence` | ✅ |
| SSE handles malformed JSON from backend without crashing | `Sse_HandlesMalformedJsonFromBackend_DoesNotCrash` | ✅ |
| SSE headers are set correctly | `Sse_HeadersAreSetCorrectly` | ✅ |
| SSE connection with invalid session ID returns 400 | `Sse_ConnectionWithInvalidSessionId_Returns400` | ✅ |
| SSE connection with expired session returns 400 | `Sse_ConnectionWithExpiredSession_Returns400` | ✅ |
| SSE connection cleanup on cancellation releases resources | `Sse_ConnectionCleanupOnCancellation_ReleasesResources` | ✅ |

### 3. Request/Response Correlation Tests

**File**: `test/Infrastructure/Sessions/McpRequestCorrelationServiceTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| RegisterPendingRequest adds to session state | `RegisterPendingRequest_AddsToSessionState` | ✅ |
| WaitForResponseAsync returns response when completed | `WaitForResponseAsync_ReturnsResponse_WhenCompleted` | ✅ |
| WaitForResponseAsync times out after RequestTimeout | `WaitForResponseAsync_TimesOut_AfterRequestTimeout` | ✅ |
| WaitForResponseAsync respects cancellation token | `WaitForResponseAsync_RespectsCancellationToken` | ✅ |
| CompleteRequest sets result on TaskCompletionSource | `CompleteRequest_SetsResult_OnTaskCompletionSource` | ✅ |
| Duplicate request ID registration fails | `RegisterPendingRequest_DuplicateId_ThrowsException` | ✅ |
| Request ID not found throws exception | `WaitForResponseAsync_RequestIdNotFound_ThrowsException` | ✅ |
| CancelRequest removes pending request | `CancelRequest_RemovesPendingRequest` | ✅ |

**File**: `test/Infrastructure/Sessions/McpResponseCorrelationServiceTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| StartAsync subscribes to existing sessions | `StartAsync_SubscribesToExistingSessions` | ✅ |
| ProcessMessage completes pending request when response received | `ProcessMessage_CompletesPendingRequest_WhenResponseReceived` | ✅ |
| ProcessMessage ignores notifications without ID | `ProcessMessage_IgnoresNotifications_WithoutId` | ✅ |
| ProcessMessage ignores requests without result or error | `ProcessMessage_IgnoresRequests_WithoutResultOrError` | ✅ |
| StopAsync cancels active subscriptions | `StopAsync_CancelsActiveSubscriptions` | ✅ |
| SubscribeToSession when service not started does nothing | `SubscribeToSession_WhenServiceNotStarted_DoesNothing` | ✅ |

### 4. Security Tests

**File**: `test/Infrastructure/Middleware/McpOriginValidationMiddlewareTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Middleware only validates /mcp paths | `InvokeAsync_OnlyValidatesMcpPaths` | ✅ |
| Middleware allows requests when no origins configured | `InvokeAsync_AllowsRequests_WhenNoOriginsConfigured` | ✅ |
| Middleware rejects unauthorized origin with 403 | `InvokeAsync_RejectsUnauthorizedOrigin_With403` | ✅ |
| Middleware logs correlation ID on rejection | `InvokeAsync_LogsCorrelationId_OnRejection` | ✅ |
| Wildcard pattern matching works correctly | `InvokeAsync_WildcardPatternMatching_WorksCorrectly` | ✅ |
| Invalid wildcard pattern doesn't crash | `InvokeAsync_InvalidWildcardPattern_DoesNotCrash` | ✅ |
| Missing Origin header is allowed | `InvokeAsync_MissingOriginHeader_IsAllowed` | ✅ |
| Exact match is case-insensitive | `InvokeAsync_ExactMatch_IsCaseInsensitive` | ✅ |

**File**: `test/HttpApi/McpStreamableControllerCorsTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Mcp-Session-Id header is present in response | `McpSessionId_Header_IsPresent_InResponse` | ✅ |
| Mcp-Session-Id header is exposed in CORS | `McpSessionId_Header_IsExposed_InCors` | ✅ |
| CORS development allows any origin | `CORS_Development_AllowsAnyOrigin` | ✅ |
| CORS production restricts to allowed origins | `CORS_Production_RestrictsToAllowedOrigins` | ✅ |
| Origin validation rejects unauthorized origins with correlation ID | `OriginValidation_RejectsUnauthorizedOrigins_WithCorrelationId` | ✅ |
| Environment variable MCP_ALLOWED_ORIGINS is parsed | `EnvironmentVariable_MCP_ALLOWED_ORIGINS_IsParsed` | ✅ |
| Environment variable MCP_EXPOSE_HEADERS is parsed | `EnvironmentVariable_MCP_EXPOSE_HEADERS_IsParsed` | ✅ |
| Exposed headers always includes Mcp-Session-Id even if not in config | `ExposedHeaders_AlwaysIncludesMcpSessionId_EvenIfNotInConfig` | ✅ |
| Origin validation with wildcard patterns works correctly | `OriginValidation_WithWildcardPatterns_WorksCorrectly` | ✅ |
| Origin validation is case-insensitive | `OriginValidation_IsCaseInsensitive` | ✅ |
| CORS preflight works correctly | `CORS_Preflight_WorksCorrectly` | ✅ |
| CORS headers in actual requests (not just preflight) | `CORS_Headers_InActualRequests_NotJustPreflight` | ✅ |
| Origin validation bypasses non-/mcp endpoints | `OriginValidation_BypassesNonMcpEndpoints` | ✅ |
| Origin validation missing Origin header is allowed | `OriginValidation_MissingOriginHeader_IsAllowed` | ✅ |

### 5. Tolerant Mode Tests

**File**: `test/HttpApi/McpStreamableControllerTolerantModeTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Tolerant mode off requires session header | `TolerantMode_Off_RequiresSessionHeader` | ✅ |
| Tolerant mode on auto-creates session for non-initialize requests | `TolerantMode_On_AutoCreatesSession_ForNonInitializeRequests` | ✅ |
| Tolerant mode returns session ID in response header | `TolerantMode_On_ReturnsSessionId_InResponseHeader` | ✅ |
| Tolerant mode session can be reused | `TolerantMode_On_SessionCanBeReused` | ✅ |
| Tolerant mode configuration is read correctly | `TolerantMode_Configuration_IsReadCorrectly` | ✅ |

### 6. Backward Compatibility Tests (Legacy /sse Endpoint)

**File**: `test/HttpApi/LegacySseCompatibilityTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| GET /sse returns 200 OK with text/event-stream | `Get_Sse_Returns200_WithTextEventStream` | ✅ |
| GET /sse establishes SSE connection and receives messages | `Get_Sse_EstablishesConnection_ReceivesMessages` | ✅ |
| POST /sse accepts JSON-RPC and forwards to backend | `Post_Sse_AcceptsJsonRpc_ForwardsToBackend` | ✅ |
| /sse endpoints do NOT require Mcp-Session-Id header | `Sse_Endpoints_DoNotRequireMcpSessionIdHeader` | ✅ |
| /sse endpoints handle Mcp-Session-Id header if provided | `Sse_Endpoints_HandleMcpSessionIdHeader_IfProvided` | ✅ |
| /sse and /mcp endpoints can coexist | `Sse_And_Mcp_Endpoints_CanCoexist` | ✅ |
| /sse endpoints use default backend when none specified | `Sse_Endpoints_UseDefaultBackend_WhenNoneSpecified` | ✅ |
| /sse endpoints use dispatcher fallback path (no session registry required) | `Sse_Endpoints_UseDispatcherFallbackPath_NoSessionRegistryRequired` | ✅ |
| Origin validation for /mcp does NOT affect /sse | `OriginValidation_ForMcp_DoesNotAffectSse` | ✅ |
| SSE connection coordinator policy is deterministic | `SseConnectionCoordinator_OneActiveConnectionPerBackend_IsDeterministic` | ✅ |
| SSE disconnect releases resources (no stuck tasks) | `Sse_Disconnect_ReleasesResources_NoStuckTasks` | ✅ |

**SSE Connection Policy**: Based on current implementation (`SseConnectionCoordinator.TryAcquire` always returns `true`), multiple SSE connections to the same backend are **allowed**. This behavior is documented and tested.

### 7. Negative Tests

**File**: `test/HttpApi/McpStreamableControllerNegativeTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Malformed JSON returns 400 | `Post_MalformedJson_Returns400` | ✅ |
| Missing JSON payload returns 400 | `Post_MissingPayload_Returns400` | ✅ |
| Invalid JSON-RPC format (missing jsonrpc field) | `Post_InvalidJsonRpcFormat_MissingJsonRpcField_ReturnsError` | ✅ |
| Missing method field returns error | `Post_MissingMethodField_ReturnsError` | ✅ |
| Invalid backend name returns 404 | `Post_InvalidBackendName_Returns404` | ✅ |
| POST without session header (non-tolerant) returns 400 | `Post_WithoutSessionHeader_NonTolerant_Returns400` | ✅ |
| GET without session header returns 400 | `Get_WithoutSessionHeader_Returns400` | ✅ |
| DELETE without session header returns 400 | `Delete_WithoutSessionHeader_Returns400` | ✅ |
| Invalid JSON-RPC id type (non-string/number) handles gracefully | `Post_InvalidJsonRpcIdType_NonStringOrNumber_HandlesGracefully` | ✅ |
| Invalid session ID returns 400 | `Get_InvalidSessionId_Returns400` | ✅ |
| Invalid Content-Type handles gracefully | `Post_InvalidContentType_HandlesGracefully` | ✅ |
| Empty JSON object handles gracefully | `Post_EmptyJsonObject_HandlesGracefully` | ✅ |
| Invalid JSON structure handles gracefully | `Post_InvalidJsonStructure_HandlesGracefully` | ✅ |
| Expired session returns 400 | `Get_ExpiredSession_Returns400` | ✅ |

### 8. Deterministic Concurrency & Race Condition Tests

**File**: `test/HttpApi/McpConcurrencyTests.cs`

| Scenario | Test Method | Status |
|----------|-------------|--------|
| Concurrent GetOrCreate with same session ID creates single backend | `SessionRegistry_ConcurrentGetOrCreate_SameSessionId_CreatesSingleBackend` | ✅ |
| Concurrent requests with unique IDs return correct responses | `Correlation_ConcurrentRequests_UniqueIds_ReturnCorrectResponses` | ✅ |
| Duplicate in-flight ID is rejected deterministically | `Correlation_DuplicateInflightId_IsRejectedDeterministically` | ✅ |
| DELETE during inflight request cleans up pending | `Session_DeleteDuringInflightRequest_CleansUpPending` | ✅ |
| SSE single subscriber policy is deterministic | `SSE_SingleSubscriberPolicy_IsDeterministic` | ✅ |
| Multiple clients create different sessions concurrently (no interference) | `MultipleClients_CreateDifferentSessions_Concurrently_NoInterference` | ✅ |
| Concurrent requests to different sessions are isolated | `ConcurrentRequests_ToDifferentSessions_AreIsolated` | ✅ |
| SSE connection established while session terminated returns 400 | `SSE_ConnectionEstablished_WhileSessionTerminated_Returns400` | ✅ |

## Enforced Concurrency Semantics

### Session Registry
- **Semantic**: Concurrent `GetOrCreate` with same `Mcp-Session-Id` must return same session and create exactly ONE backend instance.
- **Test**: `SessionRegistry_ConcurrentGetOrCreate_SameSessionId_CreatesSingleBackend`
- **Implementation**: Uses `ConcurrentDictionary.GetOrAdd` to ensure atomicity.

### JSON-RPC ID
- **Semantic**: Duplicate in-flight `id` within same session must be rejected deterministically (NOT last-wins).
- **Test**: `Correlation_DuplicateInflightId_IsRejectedDeterministically`
- **Implementation**: `PendingRequests.TryAdd` returns `false` for duplicates, handled gracefully.

### SSE Connection Policy
- **Semantic**: Current implementation allows multiple SSE connections per session/backend.
- **Test**: `SSE_SingleSubscriberPolicy_IsDeterministic`
- **Behavior**: `SseConnectionCoordinator.TryAcquire` always returns `true` (multiple connections allowed).
- **Documentation**: This behavior is documented in `TEST_MATRIX.md` and can be changed if needed.

### DELETE During Inflight
- **Semantic**: DELETE during inflight request must complete with controlled error/cancel and clean up pending requests.
- **Test**: `Session_DeleteDuringInflightRequest_CleansUpPending`
- **Implementation**: Session termination cancels pending requests and disposes backend.

## Legacy /sse Endpoint Guarantees

1. **Does NOT require Mcp-Session-Id header** - Backward compatibility maintained.
2. **Origin validation does NOT apply** - Middleware only validates `/mcp` paths.
3. **SSE connection policy** - Multiple connections allowed per backend (current behavior).
4. **Uses dispatcher fallback path** - No session registry required.

## Coverage Matrix

| Component | Unit Tests | Integration Tests | Negative Tests | Concurrency Tests |
|-----------|------------|-------------------|----------------|-------------------|
| McpSessionRegistry | ✅ | ✅ | - | ✅ |
| McpRequestCorrelationService | ✅ | ✅ | - | ✅ |
| McpResponseCorrelationService | ✅ | ✅ | - | - |
| McpStreamableController | - | ✅ | ✅ | ✅ |
| McpOriginValidationMiddleware | ✅ | ✅ | ✅ | - |
| GatewayEndpoints (/sse) | - | ✅ | - | ✅ |
| CORS Configuration | - | ✅ | ✅ | - |

## Known Limitations

1. **Real Backend Processes**: Cannot test actual stdio backend processes without external dependencies. Use `FakeBackend` mocks.
2. **Network Timeouts**: Cannot test actual network-level timeouts without network simulation. Test timeout logic via `FakeBackendWithDelay`.
3. **Load Testing**: Performance/load tests are out of scope (focus on correctness, not performance).
4. **External Dependencies**: Tests assume no external services (databases, message queues, etc.) are required.
5. **SSE Connection Policy**: Current implementation allows multiple connections. If single-connection policy is desired, `SseConnectionCoordinator` needs to be updated.

## Test Helpers

- `FakeBackend` - Basic fake backend for HTTP API tests
- `FakeBackendForConcurrency` - Channel-based fake backend for concurrency tests
- `FakeBackendFactoryForConcurrency` - Factory with CreatedCount tracking
- `FakeBackendWithDelay` - Backend that delays responses for timeout testing
- `FakeBackendWithErrors` - Backend that sends error responses
- `SseEventReader` - Helper to parse SSE events reliably
- `TestLogger` - Captures log messages for assertion (verify no secrets)
- `GatewayWebApplicationFactory` - Test factory for HTTP API tests

## Success Criteria

- ✅ All tests pass (`dotnet test`)
- ✅ Total test execution time < 30 seconds
- ✅ No secrets logged (verified via assertions)
- ✅ Coverage includes all Phase 1 components
- ✅ Negative tests cover malformed input, missing headers, wrong origin
- ✅ Concurrent tests cover race conditions with deterministic behavior
- ✅ Backward compatibility verified for /sse endpoints (legacy behavior locked in)
- ✅ Concurrency semantics documented and enforced
- ✅ Minimal production fixes applied only if tests expose real issues
