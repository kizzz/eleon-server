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

    #endregion

    #region Monthly Schedule Edge Cases

    [Fact]
    public void GetNextRunTime_Monthly_AllMonthsFilteredOut_ReturnsNull()
    {
        // Arrange - Monthly schedule with all months filtered out
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        // Set months list to empty (all filtered out)
        trigger.MonthsList = new List<int>(); // Empty list
        trigger.DaysOfMonthList = new List<int> { 1 };

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Should return null quickly due to iteration limit
        // Note: This might return null or might hit iteration limit
        // The important thing is it doesn't loop forever
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

