using System;
using FluentAssertions;
using Xunit;
using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Helpers;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.Entities;
using Common.Module.Constants;
using Volo.Abp.Validation;

namespace JobScheduler.Module.Domain.Helpers;

/// <summary>
/// Edge case tests for trigger date calculations
/// </summary>
public class TriggerDateHelperEdgeCasesTests
{
    [Fact]
    public void GetNextRunTime_LeapYear_February29_HandlesCorrectly()
    {
        // Arrange - Leap year Feb 29
        var leapYearDate = new DateTime(2024, 2, 29, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(leapYearDate)
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
    public void GetNextRunTime_NonLeapYear_February29_HandlesCorrectly()
    {
        // Arrange - Non-leap year, Feb 29 doesn't exist
        var startDate = new DateTime(2023, 2, 28, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2023, 2, 28, 11, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Day.Should().Be(1); // March 1
        result.Value.Month.Should().Be(3);
    }

    [Fact]
    public void GetNextRunTime_MonthBoundary_LastDayOfMonth_HandlesCorrectly()
    {
        // Arrange - Last day of month
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
        result.Value.Day.Should().Be(1); // Feb 1
        result.Value.Month.Should().Be(2);
    }

    [Fact]
    public void GetNextRunTime_MonthBoundary_31stDayInShortMonth_HandlesCorrectly()
    {
        // Arrange - Monthly on 31st, but some months don't have 31 days
        var startDate = new DateTime(2024, 1, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new System.Collections.Generic.List<int> { 31 };

        var now = new DateTime(2024, 2, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(3); // Skip February, go to March
        result.Value.Day.Should().Be(31);
    }

    [Fact]
    public void GetNextRunTime_YearBoundary_YearTransition_HandlesCorrectly()
    {
        // Arrange - Daily schedule crossing year boundary
        var startDate = new DateTime(2023, 12, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2023, 12, 31, 11, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Year.Should().Be(2024);
        result.Value.Day.Should().Be(1);
        result.Value.Month.Should().Be(1);
    }

    [Fact]
    public void GetNextRunTime_VeryLargeDates_FarFuture_HandlesCorrectly()
    {
        // Arrange - Very far future date
        var farFuture = new DateTime(2099, 12, 31, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(farFuture)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().Be(farFuture);
    }

    [Fact]
    public void GetNextRunTime_VerySmallIntervals_FiveMinutes_HandlesCorrectly()
    {
        // Arrange - 5-minute minimum interval
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

        var now = new DateTime(2024, 1, 1, 10, 2, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: Next repeat must be strictly > now (10:02)
        // First repeat is 10:00 + 5 min = 10:05, which is > 10:02
        result.Should().NotBeNull();
        result.Value.Minute.Should().Be(5); // 10:05 is the first repeat after 10:00
        result.Value.Should().BeAfter(now); // CONTRACT: Must be strictly > now
    }

    [Fact]
    public void GetNextRunTime_VeryLargeIntervals_Years_HandlesCorrectly()
    {
        // Arrange - Yearly intervals
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(365, TimeUnit.Days)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: Next repeat must be strictly > now (2024-06-01)
        // Major occurrence is daily, so next day is 2024-06-01 10:00
        // But repeat interval is 365 days, so next repeat would be 2025-01-01 (if within window)
        // OR next major occurrence (next day = 2024-06-01 10:00)
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now); // CONTRACT: Must be strictly > now
        // Could be next day (2024-06-01) or next yearly repeat (2025-01-01)
        if (result.Value.Year == 2025)
        {
            result.Value.Month.Should().Be(1);
            result.Value.Day.Should().Be(1);
        }
        else
        {
            result.Value.Year.Should().Be(2024);
            result.Value.Month.Should().Be(6);
            result.Value.Day.Should().Be(1);
        }
    }

    [Fact]
    public void GetNextRunTime_NullExpiry_NoExpiry_HandlesCorrectly()
    {
        // Arrange - No expiry date
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(null)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ValidateTriggerDateProperties_InvalidDateCombination_StartGreaterThanExpire_ThrowsException()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        // Act & Assert
        var action = () => TriggerDateHelper.ValidateTriggerDateProperties(trigger);
        action.Should().Throw<AbpValidationException>()
            .WithMessage("*ExpireUtc must be greater than StartUtc*");
    }

    [Fact]
    public void TrimToMinute_DateTimeWithSeconds_TrimsCorrectly()
    {
        // Arrange
        var dateWithSeconds = new DateTime(2024, 1, 1, 10, 30, 45, 123, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.TrimToMinute(dateWithSeconds);

        // Assert
        result.Second.Should().Be(0);
        result.Millisecond.Should().Be(0);
        result.Hour.Should().Be(10);
        result.Minute.Should().Be(30);
    }

    [Fact]
    public void TrimToMinute_NullDateTime_ReturnsNull()
    {
        // Arrange
        DateTime? nullDate = null;

        // Act
        var result = TriggerDateHelper.TrimToMinute(nullDate);

        // Assert
        result.Should().BeNull();
    }

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

        var now = new DateTime(2024, 1, 2, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_NullTrigger_ReturnsNull()
    {
        // Arrange
        TriggerEntity trigger = null;

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger);

        // Assert
        result.Should().BeNull();
    }
}

