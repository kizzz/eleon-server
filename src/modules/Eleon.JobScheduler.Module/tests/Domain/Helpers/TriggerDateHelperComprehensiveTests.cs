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
/// Comprehensive test coverage for TriggerDateHelper enforcing production-grade scheduling contract.
///
/// CONTRACT RULES (enforced by all tests):
/// 1. Monotonicity: NextRunUtc MUST be strictly > max(NowUtc, LastRunUtc) when LastRunUtc exists; else NextRunUtc > NowUtc.
/// 2. Expiry: If ExpireUtc is provided, NextRunUtc MUST be strictly < ExpireUtc. If NextRunUtc >= ExpireUtc => return null.
/// 3. OneTime: If LastRunUtc is null and NowUtc < StartUtc: return StartUtc (subject to expiry). If LastRunUtc is null and NowUtc >= StartUtc: return null. If LastRunUtc is set: return null. Never return DateTime.MaxValue.
/// 4. Repeats: Next repeat MUST be strictly > max(NowUtc, LastRunUtc). Never return the base time itself.
/// 5. Weekly: "Next Monday" after a Monday means +7 days, not the same day.
/// 6. Monthly: Skip months without requested day. Next occurrence must be strictly after fromExclusive.
///
/// Clock Skew Handling:
/// When now < lastRun (clock skew scenario), we still enforce monotonicity: nextRun > lastRun.
/// This prevents duplicate scheduling even when system clocks are out of sync.
/// </summary>
public class TriggerDateHelperComprehensiveTests
{
    #region OneTime Schedule Tests

    [Fact]
    public void GetNextRunTime_OneTime_BeforeStartUtc_NoLastRun_ReturnsStartUtc()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns StartUtc when now < StartUtc and LastRun is null (never MaxValue)
        result.Should().Be(startDate);
    }

    [Fact]
    public void GetNextRunTime_OneTime_AtStartUtc_NoLastRun_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns null when now >= StartUtc and LastRun is null (no catch-up)
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTime_AfterStartUtc_NoLastRun_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 20, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns null when now >= StartUtc and LastRun is null (no catch-up)
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTime_AfterStartUtc_WithLastRun_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 15, 10, 5, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 20, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTime_WithExpiry_BeforeExpiry_ReturnsStartUtc()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 20, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns StartUtc when now < StartUtc (subject to expiry)
        // Expiry is enforced: if StartUtc >= ExpireUtc, return null
        if (startDate >= expireDate)
        {
            result.Should().BeNull(); // Expired before start
        }
        else
        {
            result.Should().Be(startDate); // StartUtc is valid
        }
    }

    [Fact]
    public void GetNextRunTime_OneTime_WithExpiry_AfterExpiry_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 20, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 25, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns null when now >= StartUtc and LastRun is null (no catch-up)
        result.Should().BeNull();
    }

    #endregion

    #region LastRun Edge Cases - Daily

    [Fact]
    public void GetNextRunTime_Daily_LastRunBeforeNow_CalculatesFromNow()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc); // Now is after LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(10); // Should calculate from now, not LastRun
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Daily_LastRunAfterNow_CalculatesFromLastRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 8, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc); // Now is before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(9); // Next day after LastRun (day 8)
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Daily_LastRunEqualsNow_CalculatesNextDay()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc); // Now equals LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(6); // Next day
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Daily_LastRunBeforeExpectedLastRun_IgnoresLastRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2023, 12, 25, 10, 0, 0, DateTimeKind.Utc); // Before StartUtc
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(5); // Should calculate from now, ignoring old LastRun
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Daily_LastRunWithPeriodGreaterThanOne_CalculatesCorrectly()
    {
        // Arrange - Every 3 days
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 7, 10, 0, 0, DateTimeKind.Utc); // Day 7 (1, 4, 7, 10...)
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(3)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc); // Now is before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(10); // Next in sequence after day 7 (1, 4, 7, 10...)
        result.Value.Hour.Should().Be(10);
    }

    #endregion

    #region LastRun Edge Cases - Weekly

    [Fact]
    public void GetNextRunTime_Weekly_LastRunAfterNow_CalculatesFromLastRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var lastRun = new DateTime(2024, 1, 8, 10, 0, 0, DateTimeKind.Utc); // Next Monday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc); // Friday, before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(15); // Next Monday after LastRun (day 8)
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
    }

    [Fact]
    public void GetNextRunTime_Weekly_LastRunBeforeNow_MultipleDays_CalculatesFromNow()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 3, 10, 0, 0, DateTimeKind.Utc); // Wednesday
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1, 3, 5 }; // Mon, Wed, Fri

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc); // Wednesday, after LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(10); // Should calculate from now
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Wednesday);
    }

    #endregion

    #region LastRun Edge Cases - Monthly

    [Fact]
    public void GetNextRunTime_Monthly_LastRunAfterNow_DaysOfMonth_CalculatesFromLastRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 2, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 15 };

        var now = new DateTime(2024, 2, 1, 9, 0, 0, DateTimeKind.Utc); // Before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(3); // Next month after LastRun
        result.Value.Day.Should().Be(15);
    }

    [Fact]
    public void GetNextRunTime_Monthly_LastRunAfterNow_LastDayOfMonth_CalculatesFromLastRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 2, 29, 10, 0, 0, DateTimeKind.Utc); // Leap year
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthLast = true;

        var now = new DateTime(2024, 2, 15, 9, 0, 0, DateTimeKind.Utc); // Before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(3); // Next month
        result.Value.Day.Should().Be(31); // Last day of March
    }

    #endregion

    #region LastRun with Repeat Intervals

    [Fact]
    public void GetNextRunTime_Daily_LastRunAfterNow_WithRepeat_CalculatesFromLastRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 5, 0, DateTimeKind.Utc); // 5 minutes into the day
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 10, 2, 0, DateTimeKind.Utc); // 2 minutes after start, before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: Next repeat MUST be strictly > max(now, lastRun)
        // Since lastRun (10:05) > now (10:02), minNextRun = 10:05
        // Next repeat must be > 10:05, so it should be 10:10 (10:05 + 5 min interval)
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        result.Value.Should().BeAfter(lastRun); // CONTRACT: Must be strictly > LastRun
        if (result.Value.Day == 5 && result.Value.Hour == 10)
        {
          // CONTRACT: Never return base time (10:05) - first repeat is base + interval = 10:10
          result.Value.Minute.Should().Be(10); // 10:05 + 5 min = 10:10
        }
        else
        {
          // Might return next day if repeat window expired
          result.Value.Day.Should().Be(6);
        }
    }

    [Fact]
    public void GetNextRunTime_Daily_LastRunBeforeNow_WithRepeat_CalculatesFromNow()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 5, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes)
            .WithRepeatDuration(60, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 10, 12, 0, DateTimeKind.Utc); // After LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        // Should calculate from now, so next repeat would be 10:15
        result.Value.Minute.Should().Be(15);
        result.Value.Hour.Should().Be(10);
    }

    #endregion

    #region Expiry Edge Cases with LastRun

    [Fact]
    public void GetNextRunTime_Daily_LastRunAfterNow_WithExpiry_BeforeExpiry_ReturnsNextRun()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 20, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 8, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(9); // Next day after LastRun
        result.Value.Should().BeBefore(expireDate);
    }

    [Fact]
    public void GetNextRunTime_Daily_LastRunAfterNow_WithExpiry_AfterExpiry_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 8, 10, 0, 0, DateTimeKind.Utc); // Expiry on same day as LastRun
        var lastRun = new DateTime(2024, 1, 8, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: When LastRun is day 8 and expiry is day 8,
        // calculating from LastRun gives next run = day 9
        // Expiry is enforced: if result >= ExpireUtc, return null
        // Day 9 > ExpireUtc (day 8), so should return null
        result.Should().BeNull();
    }

    #endregion

    #region Complex LastRun Scenarios

    [Fact]
    public void GetNextRunTime_Daily_LastRunExactlyAtExpectedLastRun_CalculatesNormally()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Same as StartUtc
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(5); // Should calculate from now
        result.Value.Hour.Should().Be(10);
    }

    [Fact]
    public void GetNextRunTime_Weekly_LastRunFarInFuture_CalculatesFromLastRun()
    {
        // Arrange
        // March 1, 2024 is Friday, so we need a Monday for the test
        // March 4, 2024 is Monday
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Monday
        var lastRun = new DateTime(2024, 3, 4, 10, 0, 0, DateTimeKind.Utc); // Monday, 2 months in future
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Weekly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfWeekList = new List<int> { 1 }; // Monday

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: When LastRun is Monday March 4 and calculating next Monday,
        // next should be March 11 (Monday, +7 days), not March 4 itself
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(3);
        result.Value.Day.Should().Be(11); // Next Monday after March 4 = March 11
        result.Value.DayOfWeek.Should().Be(DayOfWeek.Monday);
        result.Value.Should().BeAfter(lastRun); // CONTRACT: Must be strictly > LastRun
    }

    [Fact]
    public void GetNextRunTime_Monthly_LastRunInShortMonth_31stDay_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 3, 31, 10, 0, 0, DateTimeKind.Utc); // March has 31 days
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 31 };

        var now = new DateTime(2024, 2, 15, 9, 0, 0, DateTimeKind.Utc); // Before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: When calculating from LastRun (March 31), the monthly logic
        // correctly skips April (which doesn't have 31 days) and goes to May
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(5); // May has 31 days (correctly skips April)
        result.Value.Day.Should().Be(31);
        result.Value.Should().BeAfter(lastRun); // CONTRACT: Must be strictly > LastRun
    }

    #endregion

    #region Disabled Trigger Tests

    [Fact]
    public void GetNextRunTime_DisabledTrigger_ReturnsNull()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(false)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_NullTrigger_ReturnsNull()
    {
        // Act
        var result = TriggerDateHelper.GetNextRunTime(null);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Very Large Intervals with LastRun

    [Fact]
    public void GetNextRunTime_Daily_LastRunWithYearlyRepeat_CalculatesCorrectly()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc); // 1 year later
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithRepeatTask(true)
            .WithRepeatInterval(365, TimeUnit.Days)
            .WithRepeatDuration(730, TimeUnit.Days) // 2 years duration
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc); // Before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: When LastRun (2025-01-01) > now (2024-06-01), minNextRun = LastRun
        // Next repeat must be > LastRun, so it should be 2025-01-01 + 365 days = 2026-01-01
        // (if within repeat window), or the next major occurrence
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        result.Value.Should().BeAfter(lastRun); // CONTRACT: Must be strictly > LastRun
        // The repeat window is 730 days from the major occurrence base (2025-01-01)
        // Next repeat would be 2026-01-01 (365 days after 2025-01-01), which is within the window
        // OR it could be the next major occurrence (next day after 2025-01-01 = 2025-01-02)
        // Let's verify it's after LastRun
        if (result.Value.Year == 2026)
        {
            result.Value.Month.Should().Be(1);
            result.Value.Day.Should().Be(1); // Next yearly repeat
        }
        else
        {
            // Might be next major occurrence (daily)
            result.Value.Should().BeAfter(lastRun);
        }
    }

    #endregion
}

