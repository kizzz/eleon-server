using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Helpers;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.Entities;
using Common.Module.Constants;

namespace JobScheduler.Module.Domain.Helpers;

/// <summary>
/// Special case tests for TriggerDateHelper covering edge cases identified in deep analysis.
/// </summary>
public class TriggerDateHelperSpecialCasesTests
{
    #region FindMajorOccurrenceForTime Edge Cases

    [Fact]
    public void GetNextRunTime_LastRunExactlyOnMajorOccurrenceBoundary_ReturnsNextOccurrence()
    {
        // Arrange - LastRun exactly on a daily occurrence boundary
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc); // Exactly on occurrence
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc); // Same as LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day
    }

    [Fact]
    public void GetNextRunTime_LastRunBetweenMajorOccurrences_ReturnsNextOccurrence()
    {
        // Arrange - LastRun between daily occurrences
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 15, 30, 0, DateTimeKind.Utc); // Between occurrences
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 12, 0, 0, DateTimeKind.Utc); // Before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day
        result.Value.Hour.Should().Be(10); // Same time as StartUtc
    }

    [Fact]
    public void GetNextRunTime_LastRunBeforeStartUtc_CalculatesFromNow()
    {
        // Arrange - LastRun before StartUtc
        var startDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc); // Before StartUtc
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(15); // Should calculate from now, not LastRun
    }

    [Fact]
    public void GetNextRunTime_LastRunFarInFuture_CalculatesFromLastRun()
    {
        // Arrange - LastRun far in the future
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2025, 6, 15, 10, 0, 0, DateTimeKind.Utc); // 1.5 years in future
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(16); // Next day after LastRun
    }

    [Fact]
    public void GetNextRunTime_RepeatDurationNull_DoesNotRepeat()
    {
        // Arrange - RepeatTask enabled but no duration should disable repeats
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = lastRun;

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc));
    }

    #endregion

    #region Repeat Interval Edge Cases

    [Fact]
    public void GetNextRunTime_RepeatIntervalEqualsRepeatDuration_BoundaryCondition()
    {
        // Arrange - Repeat interval equals repeat duration (boundary condition)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(60, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes) // Same as interval
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return next major occurrence since repeat window is only 60 minutes
        // and we're already 30 minutes in, next repeat would be at 11:00 which is at the boundary
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
    }

    [Fact]
    public void GetNextRunTime_RepeatIntervalVeryCloseToDuration_HandlesCorrectly()
    {
        // Arrange - Repeat interval very close to duration (59 minutes vs 60 minutes)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(59, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 1, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        // Should be able to fit one repeat (at 10:59) within the 60-minute window
        if (result.Value.Hour == 10)
        {
            result.Value.Minute.Should().Be(59);
        }
    }

    [Fact]
    public void GetNextRunTime_VerySmallRepeatInterval_HandlesCorrectly()
    {
        // Arrange - Very small repeat interval (5 seconds) with large duration
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes) // Smallest allowed is 5 minutes
            .WithRepeatDuration(120, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 2, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        result.Value.Minute.Should().Be(5); // Next 5-minute interval
    }

    [Fact]
    public void GetNextRunTime_VeryLargeRepeatInterval_HandlesCorrectly()
    {
        // Arrange - Very large repeat interval (1 year) with small duration
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(365, TimeUnit.Days)
            .WithRepeatDuration(730, TimeUnit.Days) // 2 years
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        // Could be next day or next yearly repeat
    }

    [Fact]
    public void GetNextRunTime_RepeatIntervalNotDividingEvenly_HandlesCorrectly()
    {
        // Arrange - Repeat interval that doesn't divide evenly into duration
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(7, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes) // 7 doesn't divide evenly into 60
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        // Should calculate next repeat correctly (10:07, 10:14, etc.)
    }

    [Fact]
    public void GetNextRunTime_RepeatIntervalGreaterThanDuration_FallsBackToMajorOccurrence()
    {
        // Arrange - Repeat interval greater than duration should disable repeats
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(90, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = lastRun;

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GetNextRunTime_RepeatDurationZero_DoesNotRepeat()
    {
        // Arrange - Repeat duration zero should disable repeats
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes)
            .WithRepeatDuration(0, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = lastRun;

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void GetNextRunTime_RepeatWithinWindow_ReturnsNextRepeat()
    {
        // Arrange - Repeat within window should return next repeat before next major occurrence
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 1, 10, 5, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(15, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 20, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(new DateTime(2024, 1, 1, 10, 30, 0, DateTimeKind.Utc));
    }

    #endregion

    #region Monthly Schedule Edge Cases

    [Fact]
    public void GetNextRunTime_Monthly_EmptyMonthsList_FallsBackToMask_AllMonths()
    {
        // Arrange - Monthly schedule with empty MonthsList falls back to mask
        // Empty list + mask=0 means all months (default behavior)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithMonths(0) // Mask = 0 means all months
            .WithIsEnabled(true)
            .Build();

        trigger.MonthsList = new List<int>(); // Empty list triggers mask fallback
        trigger.DaysOfMonthList = new List<int> { 1 };

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return next month (Feb 1) because mask=0 means all months
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(2);
        result.Value.Day.Should().Be(1);
    }

    [Fact]
    public void GetNextRunTime_Monthly_EmptyMonthsList_FallsBackToMask_SpecificMonths()
    {
        // Arrange - Monthly schedule with empty MonthsList falls back to mask
        // Mask selects Jan (bit 0), Apr (bit 3), Dec (bit 11)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithMonths((1 << 0) | (1 << 3) | (1 << 11)) // Jan, Apr, Dec
            .WithIsEnabled(true)
            .Build();

        trigger.MonthsList = new List<int>(); // Empty list triggers mask fallback
        trigger.DaysOfMonthList = new List<int> { 15 };

        var now = new DateTime(2024, 1, 20, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return Apr 15 (next selected month)
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(4);
        result.Value.Day.Should().Be(15);
    }

    [Fact]
    public void GetNextRunTime_Monthly_NoMonthsList_FallsBackToMask()
    {
        // Arrange - Monthly schedule with null MonthsList falls back to mask
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithMonths((1 << 0) | (1 << 5)) // Jan, Jun
            .WithIsEnabled(true)
            .Build();

        trigger.MonthsList = null; // Null triggers mask fallback
        trigger.DaysOfMonthList = new List<int> { 10 };

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return Jun 10 (next selected month)
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(6);
        result.Value.Day.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Monthly_NoCandidates_ReturnsNull()
    {
        // Arrange - Monthly schedule with no DOM and no weekday-occurrence config
        // This should return null quickly without looping
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int>(); // Empty
        trigger.DaysOfMonthLast = false;
        trigger.DaysOfWeekList = new List<int>(); // Empty
        trigger.DaysOfWeekOccurencesList = new List<int>(); // Empty
        trigger.DaysOfWeekOccurencesLast = false;

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return null because no candidates can be generated
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_Monthly_PeriodGreaterThanOne_FromExclusiveInMiddle_CalculatesCorrectly()
    {
        // Arrange - Monthly schedule with period > 1, fromExclusive in middle of period
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(3) // Every 3 months
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 15 };

        // Now is in the middle of a period (between Jan and Apr)
        var now = new DateTime(2024, 2, 20, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(4); // Next occurrence (Jan, Apr, Jul, Oct...)
        result.Value.Day.Should().Be(15);
    }

    [Fact]
    public void GetNextRunTime_Monthly_MultipleDaysOfMonth_SomeInvalidForCertainMonths_HandlesCorrectly()
    {
        // Arrange - Monthly schedule with multiple days, some invalid for certain months
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 30, 31 }; // Both days

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().BeOneOf(30, 31);
        result.Value.Month.Should().Be(1); // January has both 30 and 31
    }

    [Fact]
    public void GetNextRunTime_Monthly_LastDayOfMonthWithPeriodGreaterThanOne_CalculatesCorrectly()
    {
        // Arrange - Monthly schedule with last day of month and period > 1
        var startDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(2) // Every 2 months
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthLast = true;

        var now = new DateTime(2024, 2, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(3); // Next occurrence (Jan, Mar, May...)
        result.Value.Day.Should().Be(31); // Last day of March
    }

    [Fact]
    public void GetNextRunTime_Monthly_WeekdayOccurrencesWithPeriodGreaterThanOne_CalculatesCorrectly()
    {
        // Arrange - Monthly schedule with weekday occurrences and period > 1
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(2) // Every 2 months
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday
        trigger.DaysOfWeekOccurencesList = new List<int> { 1 }; // First occurrence

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(3); // Next occurrence (Jan, Mar, May...)
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    #endregion

    #region Weekly Schedule Edge Cases

    [Fact]
    public void GetNextRunTime_Weekly_PeriodGreaterThanOne_FromExclusiveInMiddle_CalculatesCorrectly()
    {
        // Arrange - Weekly schedule with period > 1, fromExclusive in middle of period
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(2) // Every 2 weeks
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday

        // Now is in the middle of a period (between week 0 and week 2)
        var now = new DateTime(2024, 1, 8, 9, 0, 0, DateTimeKind.Utc); // Monday of week 2

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(15); // Next Monday (2 weeks later)
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void GetNextRunTime_Weekly_MultipleDaysWithPeriodGreaterThanOne_CalculatesCorrectly()
    {
        // Arrange - Weekly schedule with multiple days per week and period > 1
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(2) // Every 2 weeks
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1, 3, 5 }; // Mon, Wed, Fri

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc); // Tuesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().BeOneOf(DayOfWeek.Wednesday, DayOfWeek.Friday);
    }

    [Fact]
    public void GetNextRunTime_Weekly_YearBoundaryCrossingWithPeriodGreaterThanOne_HandlesCorrectly()
    {
        // Arrange - Weekly schedule crossing year boundary with period > 1
        var startDate = new DateTime(2023, 12, 1, 10, 0, 0, DateTimeKind.Utc); // Friday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(2) // Every 2 weeks
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 5 }; // Friday

        var now = new DateTime(2023, 12, 31, 9, 0, 0, DateTimeKind.Utc); // Sunday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Year.Should().Be(2024);
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Friday);
    }

    [Fact]
    public void GetNextRunTime_Weekly_FromExclusiveExactlyOnTargetWeekday_ReturnsNextWeek()
    {
        // Arrange - Weekly schedule with fromExclusive exactly on target weekday
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday

        // fromExclusive is exactly on Monday at 10:00 (same as StartUtc)
        var now = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(8); // Next Monday (+7 days)
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Weekly_StartUtcOnDisallowedWeekday_ReturnsNextAllowedWeekday()
    {
        // Arrange - Weekly schedule where StartUtc is NOT on an allowed weekday
        // StartUtc is Tuesday, but schedule is Mondays only
        var startDate = new DateTime(2024, 1, 2, 10, 0, 0, DateTimeKind.Utc); // Tuesday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday only

        // Now is before StartUtc
        var now = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc); // Monday before StartUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return next allowed weekday (Monday) after StartUtc, not StartUtc itself
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Value.Day.Should().Be(8); // Next Monday after StartUtc (Jan 2)
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Weekly_EmptyDaysOfWeekList_FallsBackToMask_SpecificDays()
    {
        // Arrange - Weekly schedule with empty DaysOfWeekList falls back to mask
        // Mask selects Monday (bit 0) and Wednesday (bit 2)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithDaysOfWeek((1 << 0) | (1 << 2)) // Mon, Wed
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int>(); // Empty list triggers mask fallback

        var now = new DateTime(2024, 1, 3, 9, 0, 0, DateTimeKind.Utc); // Wednesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return next Monday (next week)
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Value.Day.Should().Be(8);
    }

    [Fact]
    public void GetNextRunTime_Weekly_NullDaysOfWeekList_FallsBackToMask()
    {
        // Arrange - Weekly schedule with null DaysOfWeekList falls back to mask
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithDaysOfWeek((1 << 4) | (1 << 6)) // Fri, Sun
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = null; // Null triggers mask fallback

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc); // Tuesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return Friday (next selected day)
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Friday);
        result.Value.Day.Should().Be(5);
    }

    [Fact]
    public void GetNextRunTime_Weekly_MaskZero_AllDays()
    {
        // Arrange - Weekly schedule with mask=0 means all days
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithDaysOfWeek(0) // Mask = 0 means all days
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int>(); // Empty list triggers mask fallback

        var now = new DateTime(2024, 1, 3, 9, 0, 0, DateTimeKind.Utc); // Wednesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return next day (Thursday) because all days are allowed
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(4); // Thursday
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Thursday);
    }

    [Fact]
    public void GetNextRunTime_Weekly_InvalidISOValues_Ignored()
    {
        // Arrange - Weekly schedule with invalid ISO values (0, 8, 14) should be ignored
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        // Invalid values: 0, 8, 14 should be ignored; valid: 1 (Monday), 3 (Wednesday)
        trigger.DaysOfWeekList = new List<int> { 0, 1, 8, 3, 14 }; // Invalid values mixed with valid

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc); // Tuesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should use only valid values (Mon, Wed), so next should be Wednesday
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Wednesday);
        result.Value.Day.Should().Be(3);
    }

    [Fact]
    public void GetNextRunTime_Monthly_WeekdayModel_EmptyDaysOfWeekList_FallsBackToMask()
    {
        // Arrange - Monthly weekday model with empty DaysOfWeekList falls back to mask
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithDaysOfWeek((1 << 0) | (1 << 2)) // Mon, Wed
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int>(); // Empty triggers mask fallback
        trigger.DaysOfWeekOccurencesList = new List<int> { 1 }; // First occurrence

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc); // Tuesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return first Wednesday of January (next selected weekday)
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Wednesday);
        result.Value.Month.Should().Be(1);
    }

    [Fact]
    public void GetNextRunTime_Monthly_WeekdayModel_MaskZero_AllWeekdays()
    {
        // Arrange - Monthly weekday model with mask=0 means all weekdays
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithDaysOfWeek(0) // Mask = 0 means all weekdays
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int>(); // Empty triggers mask fallback
        trigger.DaysOfWeekOccurencesList = new List<int> { 1 }; // First occurrence

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc); // Tuesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return first occurrence of any weekday (should be valid)
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(1);
    }

    #endregion

    #region Daily Schedule Edge Cases

    [Fact]
    public void GetNextRunTime_Daily_VeryLargePeriod_HandlesCorrectly()
    {
        // Arrange - Daily schedule with very large period (1000 days)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1000) // Every 1000 days
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = startDate.AddDays(500); // 500 days after start

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        // Should calculate next occurrence correctly (day 1000, 2000, etc.)
    }

    [Fact]
    public void GetNextRunTime_Daily_PeriodNotDividingEvenlyIntoYear_HandlesCorrectly()
    {
        // Arrange - Daily schedule with period that doesn't divide evenly into year
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(7) // Every 7 days (doesn't divide evenly into 365)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 6, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        // Should calculate next occurrence correctly
    }

    [Fact]
    public void GetNextRunTime_Daily_YearBoundaryCrossingWithPeriodGreaterThanOne_HandlesCorrectly()
    {
        // Arrange - Daily schedule crossing year boundary with period > 1
        var startDate = new DateTime(2023, 12, 28, 10, 0, 0, DateTimeKind.Utc); // Dec 28 (period 3: Dec 28, 31, Jan 3...)
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(3) // Every 3 days
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2023, 12, 31, 11, 0, 0, DateTimeKind.Utc); // After Dec 31 occurrence

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        // Should cross year boundary to next occurrence (Jan 3, 2024)
        result.Value.Year.Should().Be(2024);
        result.Value.Month.Should().Be(1);
    }

    #endregion
}

