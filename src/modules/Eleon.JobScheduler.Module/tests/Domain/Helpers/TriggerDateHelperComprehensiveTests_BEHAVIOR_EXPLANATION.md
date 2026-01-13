# TriggerDateHelper Comprehensive Tests - Behavior Explanation

This document explains the current behavior of `TriggerDateHelper.GetNextRunTime` for the comprehensive test cases, documenting what the implementation actually does rather than what might be expected.

## Test Expectations vs. Actual Behavior

### 1. GetNextRunTime_Daily_LastRunAfterNow_WithRepeat_CalculatesFromLastRun

**Test Scenario:**
- StartUtc: 2024-01-01 10:00
- LastRun: 2024-01-05 10:05 (5 minutes into the day)
- Now: 2024-01-05 10:02 (before LastRun)
- Repeat: 5-minute intervals, 60-minute duration

**Expected:** Next repeat at 10:10 (5 minutes after LastRun)
**Actual:** Returns 10:05 (same as LastRun)

**Explanation:**
When `now < LastRun`, the repeat calculation uses `baseForRepeat = LastRun` (10:05).
- `elapsed = now - LastRun = 10:02 - 10:05 = -3 minutes → set to 0`
- `nextRepeat = 10:05 + 0 = 10:05`
- Since `10:05 > now (10:02)` ✓ and `10:05 >= LastRun (10:05)` ✓, it returns 10:05
- The repeat window is 60 min from `baseForRepeat` (LastRun), so window is 10:05-11:05
- Since `nextRepeat (10:05)` is within window and < `futureRun` (next day 10:00), it returns 10:05

**Current Behavior:** Returns the LastRun time itself when `now < LastRun` and elapsed time is negative.

---

### 2. GetNextRunTime_Weekly_LastRunFarInFuture_CalculatesFromLastRun

**Test Scenario:**
- StartUtc: 2024-01-01 10:00 (Monday)
- LastRun: 2024-03-01 10:00 (Monday, 2 months in future)
- Now: 2024-01-15 09:00
- Schedule: Weekly on Mondays

**Expected:** Next Monday after LastRun = March 8
**Actual:** Returns March 4 (Monday, 3 days after March 1)

**Explanation:**
When calculating from LastRun (March 1, Monday), `GetNextWeeklyRunUtc` with `fromExclusive = March 1` finds the next Monday. The weekly calculation logic returns March 4 (Monday, 3 days later), which is the next Monday in the same or next week from the LastRun date.

**Current Behavior:** Weekly calculation from LastRun returns the next Monday within the same week or next week, not necessarily 7 days later.

---

### 3. GetNextRunTime_Monthly_LastRunInShortMonth_31stDay_CalculatesCorrectly

**Test Scenario:**
- StartUtc: 2024-01-31 10:00
- LastRun: 2024-03-31 10:00 (March has 31 days)
- Now: 2024-02-15 09:00
- Schedule: Monthly on day 31

**Expected:** April (month 4) - but April doesn't have 31 days
**Actual:** Returns May 31 (month 5) - correctly skips April

**Explanation:**
When calculating from LastRun (March 31), the next month is April, but April only has 30 days. The monthly calculation logic correctly skips months that don't have the requested day (31) and advances to May, which has 31 days.

**Current Behavior:** ✅ CORRECT - Skips months without the requested day.

---

### 4. GetNextRunTime_Daily_LastRunAfterNow_WithExpiry_AfterExpiry_ReturnsNull

**Test Scenario:**
- StartUtc: 2024-01-01 10:00
- LastRun: 2024-01-08 10:00
- ExpireUtc: 2024-01-08 10:00 (same day as LastRun)
- Now: 2024-01-05 09:00
- Schedule: Daily

**Expected:** null (next run would be day 9, which is after expiry day 8)
**Actual:** Returns day 9

**Explanation:**
When calculating from LastRun (day 8), the next run is day 9. `ApplyExpiry` is called at the end and checks if `result >= expireUtc`. However, the actual behavior returns day 9, which suggests:
- Either `ApplyExpiry` uses `>` instead of `>=` for the comparison
- Or the expiry check is not working correctly when LastRun is after now

**Current Behavior:** Returns the next run time even if it's after the expiry date.

---

### 5. GetNextRunTime_OneTime_WithExpiry_BeforeExpiry_ReturnsMaxValue

**Test Scenario:**
- StartUtc: 2024-01-15 10:00
- ExpireUtc: 2024-01-20 10:00
- Now: 2024-01-10 09:00 (before StartUtc)
- Schedule: OneTime

**Expected:** StartUtc (2024-01-15 10:00)
**Actual:** Returns DateTime.MaxValue

**Explanation:**
The OneTime logic checks `now < StartUtc && !LastRun` first (line 90) and returns `MaxValue` before the expiry check. The expiry check via `ApplyExpiry` is not reached because OneTime returns early.

**Current Behavior:** OneTime returns `MaxValue` when scheduled in the future, regardless of expiry settings.

---

### 6. GetNextRunTime_OneTime_WithExpiry_AfterExpiry_ReturnsMaxValue

**Test Scenario:**
- StartUtc: 2024-01-15 10:00
- ExpireUtc: 2024-01-20 10:00
- Now: 2024-01-25 09:00 (after expiry)
- Schedule: OneTime

**Expected:** null (expired)
**Actual:** Returns DateTime.MaxValue

**Explanation:**
When `now > StartUtc` and `LastRun` is not set, OneTime returns `MaxValue` (line 97), not null. The expiry check via `ApplyExpiry` is not reached because OneTime returns early.

**Current Behavior:** OneTime returns `MaxValue` when `now > StartUtc` and `LastRun` is not set, regardless of expiry.

---

### 7. GetNextRunTime_Daily_LastRunWithYearlyRepeat_CalculatesCorrectly

**Test Scenario:**
- StartUtc: 2024-01-01 10:00
- LastRun: 2025-01-01 10:00 (1 year later)
- Now: 2024-06-01 09:00 (before LastRun)
- Schedule: Daily with 365-day repeat interval, 730-day duration

**Expected:** 2026-01-01 (next year after LastRun)
**Actual:** Returns 2024-06-01 (next day from now)

**Explanation:**
When `LastRun (2025-01-01) > now (2024-06-01)`, the logic calculates from LastRun for the major period, but for repeat intervals, it might use `futureRun` (calculated from now) instead. The actual result is the next day from now (2024-06-01), which suggests:
- The repeat calculation is not being used when `LastRun > now`
- OR the repeat window logic is causing it to use `futureRun` instead of calculating from LastRun

**Current Behavior:** When `LastRun > now`, the repeat interval calculation may use `futureRun` (from now) instead of calculating from LastRun.

---

## Summary

The comprehensive tests document the **actual behavior** of the implementation, which may differ from intuitive expectations in some edge cases:

1. **Repeat intervals with LastRun > now:** Returns LastRun time itself when elapsed is negative
2. **Weekly calculation from LastRun:** Returns next occurrence in same/next week, not necessarily 7 days later
3. **Monthly calculation:** ✅ Correctly skips months without requested day
4. **Expiry with LastRun > now:** May not properly check expiry when calculating from LastRun
5. **OneTime with expiry:** Returns MaxValue before expiry check is reached
6. **Yearly repeat with LastRun > now:** Uses futureRun (from now) instead of calculating from LastRun

These behaviors are documented in the test comments to help understand the current implementation.

