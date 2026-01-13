# TriggerDateHelper Production-Grade Scheduling Contract

## Overview

`TriggerDateHelper.GetNextRunTime` enforces a strict production-grade scheduling contract to ensure correctness, determinism, and prevent duplicate/retroactive scheduling.

## Contract Rules

### 1. Monotonicity (No Duplicates / No Time Travel)

**Rule:** NextRunUtc MUST be strictly > max(NowUtc, LastRunUtc) when LastRunUtc exists; else NextRunUtc > NowUtc.

**Rationale:** Prevents duplicate scheduling and ensures we never schedule in the past.

**Implementation:**
- Calculate `minNextRun = max(now, lastRun)` when LastRun exists
- All calculations start from `minNextRun` (not just `now`)
- Final candidate is validated to be strictly > `minNextRun`

**Clock Skew Handling:**
When `now < lastRun` (clock skew scenario), we still enforce monotonicity: `nextRun > lastRun`.
This prevents duplicate scheduling even when system clocks are out of sync. We treat clock skew as a real scenario that must be handled safely, not ignored.

### 2. Expiry Enforcement

**Rule:** If ExpireUtc is provided, NextRunUtc MUST be strictly < ExpireUtc. If NextRunUtc >= ExpireUtc => return null.

**Rationale:** Expired triggers should not run. This must be enforced on ALL code paths.

**Implementation:**
- Expiry check is applied at the END of `GetNextRunTime`, after all calculations
- No early returns that bypass expiry check
- If `candidateNext >= ExpireUtc`, return `null`

### 3. OneTime Schedule

**Rule:**
- If LastRunUtc is null and NowUtc < StartUtc: return StartUtc (subject to expiry rule)
- If LastRunUtc is null and NowUtc >= StartUtc: return null (no catch-up)
- If LastRunUtc is set: return null (already ran)
- Never return DateTime.MaxValue as a sentinel

**Rationale:** OneTime schedules run exactly once. No catch-up for missed runs. Use nullable return type, not sentinel values.

**Implementation:**
- Early return for OneTime schedules
- Check LastRun first - if set, return null
- If now < StartUtc, return StartUtc (subject to expiry)
- If now >= StartUtc, return null (missed the window)

### 4. Repeat Intervals (Intra-Window Repetition)

**Rule:**
- Next repeat MUST be strictly > max(NowUtc, LastRunUtc)
- Never return the base time itself as a repeat
- If now is before base, the first repeat candidate is base + RepeatInterval (not base)
- Repeat window: [base, base + RepeatDuration). Any repeat candidate >= base + duration is invalid

**Rationale:** Repeats are sub-intervals within a major occurrence window. They must advance forward, never duplicate the base time.

**Implementation:**
- Determine repeat window base: most recent major occurrence <= max(now, StartUtc) OR LastRun's major occurrence period
- First repeat = base + interval (never base itself)
- Advance repeat until strictly > minNextRun
- Check if within window: repeat < base + duration
- If repeat >= next major occurrence, use next major occurrence instead

### 5. Weekly Recurrence

**Rule:** "Weekly on Monday" means next Monday strictly after a given fromExclusive time.
- If fromExclusive is a Monday at 10:00, next should be the next week's Monday at 10:00 (i.e., +7 days), not the same day/time.

**Rationale:** Prevents scheduling the same occurrence twice when fromExclusive is already on the target weekday.

**Implementation:**
- `GetNextWeeklyRunUtc` ensures candidate is strictly > fromExclusive
- If fromExclusive is already on target weekday, advance to next week (+7 days)

### 6. Monthly Recurrence with Day-of-Month

**Rule:**
- If day 31 and month has no 31, skip that month (current behavior OK)
- Next occurrence must be strictly after fromExclusive

**Rationale:** Handles variable month lengths correctly. Prevents duplicate scheduling.

**Implementation:**
- `GetNextMonthlyRunUtc` skips months without requested day
- Candidate is filtered to be strictly > fromExclusive

## Test Coverage Requirements

All tests must enforce the contract. Test cases should cover:

### A) Clock Skew with Repeats
- Scenario: `now < lastRun` (clock skew)
- Expected: Next repeat must be > lastRun, not equal to lastRun
- Test: `GetNextRunTime_Daily_LastRunAfterNow_WithRepeat_CalculatesFromLastRun`

### B) Expiry Enforcement
- Scenario: Next run would be >= ExpireUtc
- Expected: Return null
- Test: `GetNextRunTime_Daily_LastRunAfterNow_WithExpiry_AfterExpiry_ReturnsNull`

### C) OneTime Correctness
- Scenario: now < StartUtc, no LastRun
- Expected: Return StartUtc (not MaxValue)
- Scenario: now >= StartUtc, no LastRun
- Expected: Return null (no catch-up)
- Scenario: LastRun is set
- Expected: Return null (already ran)

### D) Weekly Correctness
- Scenario: fromExclusive is already on target weekday
- Expected: Next occurrence is +7 days (next week), not same day
- Test: Use correct calendar dates (verify actual weekday matches test assumption)

### E) Yearly-Like Repeats
- Scenario: LastRun is in future relative to now
- Expected: Next repeat must be strictly > LastRun (unless expiry blocks)
- Test: `GetNextRunTime_Daily_LastRunWithYearlyRepeat_CalculatesCorrectly`

## Implementation Notes

### Centralized Candidate Selection

The implementation follows this pattern:
1. Compute `candidateNext` (nullable)
2. Enforce monotonicity: `candidate > max(now, lastRun?)`
3. Enforce expiry: `candidate < expireUtc`
4. Return candidate or null

### Return Type

The method returns `DateTime?` (nullable). This is preferred over using `DateTime.MaxValue` as a sentinel value because:
- Null clearly indicates "no next run"
- MaxValue is a valid date that could theoretically be used
- Nullable types are more expressive and type-safe

### Date Calculations

All date calculations use:
- `DateTimeKind.Utc` explicitly
- Fixed `DateTime` values in tests (no `DateTime.Now` or `DateTime.UtcNow`)
- Calendar-aware logic for month boundaries, leap years, etc.

## Migration Notes

The previous implementation had several edge cases that violated the contract:
- OneTime returned `MaxValue` instead of `StartUtc` or `null`
- Repeats could return the base time itself (violating monotonicity)
- Weekly calculation could return same day when fromExclusive was on target weekday
- Expiry was not enforced on all code paths

All these issues have been fixed to comply with the contract.

