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
/// Advanced trigger date calculation tests
/// </summary>
public class TriggerDateHelperAdvancedTests
{
    [Fact]
    public void GetNextRunTime_DailySchedule_LeapYearFebruary_HandlesCorrectly()
    {
        // Arrange - Daily schedule starting on Feb 29 in leap year
        var leapYear = new DateTime(2024, 2, 29, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(leapYear)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 3, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
    }

    [Fact]
    public void GetNextRunTime_DailySchedule_MonthBoundary_HandlesCorrectly()
    {
        // Arrange - Daily schedule crossing month boundary
        var startDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 31, 11, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        result.Value.Day.Should().Be(1); // Next day is Feb 1
        result.Value.Month.Should().Be(2);
    }

    [Fact]
    public void GetNextRunTime_DailySchedule_PeriodGreaterThanOne_CalculatesCorrectly()
    {
        // Arrange - Every 3 days
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(3)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc); // Day 5, should next be day 7

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(7); // Jan 1, 4, 7, 10...
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_WeeklySchedule_MultipleDaysPerWeek_CalculatesCorrectly()
    {
        // Arrange - Monday, Wednesday, Friday
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1, 3, 5 }; // Mon, Wed, Fri (ISO)

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc); // Tuesday

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void GetNextRunTime_WeeklySchedule_PeriodGreaterThanOne_CalculatesCorrectly()
    {
        // Arrange - Every 2 weeks
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(2)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday

        var now = new DateTime(2024, 1, 8, 9, 0, 0, DateTimeKind.Utc); // Next Monday (week 2)

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(15); // 2 weeks later
    }

    [Fact]
    public void GetNextRunTime_MonthlySchedule_DaysOfMonth_CalculatesCorrectly()
    {
        // Arrange - 1st and 15th of each month
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 1, 15 };

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(15);
        result.Value.Month.Should().Be(1);
    }

    [Fact]
    public void GetNextRunTime_MonthlySchedule_LastDayOfMonth_HandlesCorrectly()
    {
        // Arrange - Last day of month
        var startDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthLast = true;

        var now = new DateTime(2024, 2, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(29); // Feb 2024 is leap year
        result.Value.Month.Should().Be(2);
    }

    [Fact]
    public void GetNextRunTime_MonthlySchedule_DaysOfWeekWithOccurrences_CalculatesCorrectly()
    {
        // Arrange - First Monday of each month
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday (ISO)
        trigger.DaysOfWeekOccurencesList = new List<int> { 1 }; // First occurrence

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(2);
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Value.Day.Should().BeLessThanOrEqualTo(7); // First Monday of February
    }

    [Fact]
    public void GetNextRunTime_MonthlySchedule_LastFridayOfMonth_CalculatesCorrectly()
    {
        // Arrange - Last Friday of each month
        var startDate = new DateTime(2024, 1, 26, 10, 0, 0, DateTimeKind.Utc); // Last Friday of Jan
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 5 }; // Friday (ISO)
        trigger.DaysOfWeekOccurencesLast = true;

        var now = new DateTime(2024, 2, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(2);
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Friday);
        result.Value.Day.Should().BeGreaterThan(20); // Last Friday
    }

    [Fact]
    public void GetNextRunTime_OneTimeSchedule_AfterLastRun_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithLastRun(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTimeSchedule_BeforeStartUtc_ReturnsStartUtc()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2023, 12, 31, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns StartUtc when now < StartUtc and LastRun is null (never MaxValue)
        result.Should().Be(startDate);
    }

    [Fact]
    public void GetNextRunTime_RepeatInterval_WithinWindow_CalculatesCorrectly()
    {
        // Arrange - Daily with 5-minute repeats for 1 hour
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 12, 0, DateTimeKind.Utc); // 12 minutes after start

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: Next repeat must be strictly > now (10:12)
        // First repeat is 10:00 + 5 min = 10:05, but that's before now
        // Next repeat after now is 10:15 (10:05 + 3 intervals = 10:15)
        result.Should().NotBeNull();
        result.Value.Minute.Should().Be(15); // Next 5-minute interval after 10:12
        result.Value.Should().BeAfter(now); // CONTRACT: Must be strictly > now
    }

    [Fact]
    public void GetNextRunTime_RepeatInterval_ExceedingNextMajor_ReturnsNextMajor()
    {
        // Arrange - Daily with repeats, but next repeat exceeds next day
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(60, TimeUnit.Minutes) // 1 hour intervals
            .WithRepeatDuration(120, TimeUnit.Minutes) // 2 hour window
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 23, 30, 0, DateTimeKind.Utc); // Late in the day

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(2); // Next day
    }

    [Fact]
    public void GetNextRunTime_ExpiryDate_AfterExpiry_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 2, 1, 9, 0, 0, DateTimeKind.Utc); // After expiry

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_LastRunGreaterThanExpected_HandlesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc)) // Last run on day 5
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 3, 9, 0, 0, DateTimeKind.Utc); // Now is day 3

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(6); // Next after last run
    }

    [Fact]
    public void GetNextRunTime_StartUtcInFuture_ReturnsStartUtc()
    {
        // Arrange
        var futureStart = new DateTime(2024, 12, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(futureStart)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().Be(futureStart);
    }

    [Fact]
    public void GetNextRunTime_ComplexMonthlyWithRepeat_CalculatesCorrectly()
    {
        // Arrange - Monthly on 1st and 15th, with 30-minute repeats for 2 hours
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(30, TimeUnit.Minutes)
            .WithRepeatDuration(120, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 1, 15 };

        var now = new DateTime(2024, 1, 1, 10, 45, 0, DateTimeKind.Utc); // 45 minutes after start

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Minute.Should().Be(0); // Next 30-minute interval (11:00)
    }
}

