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
/// Boundary condition tests for TriggerDateHelper covering exact equality scenarios.
/// </summary>
public class TriggerDateHelperBoundaryTests
{
    #region fromExclusive Boundary Conditions

    [Fact]
    public void GetNextRunTime_FromExclusiveExactlyEqualToStartUtc_ReturnsNextOccurrence()
    {
        // Arrange - fromExclusive exactly equal to StartUtc
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Exactly equal to StartUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(startDate);
        result.Value.Day.Should().Be(2); // Next day
    }

    [Fact]
    public void GetNextRunTime_FromExclusiveExactlyEqualToExpireUtc_ReturnsNull()
    {
        // Arrange - fromExclusive exactly equal to ExpireUtc
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc); // Exactly equal to ExpireUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Next run would be day 11, which is >= ExpireUtc (day 10), so should return null
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_FromExclusiveOneTickAfterExpireUtc_ReturnsNull()
    {
        // Arrange - fromExclusive one tick after ExpireUtc
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = expireDate.AddTicks(1); // One tick after ExpireUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_FromExclusiveExactlyEqualToMajorOccurrence_ReturnsNextOccurrence()
    {
        // Arrange - fromExclusive exactly equal to a major occurrence
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc); // Major occurrence
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc); // Exactly equal to LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day
    }

    #endregion

    #region LastRun Boundary Conditions

    [Fact]
    public void GetNextRunTime_LastRunExactlyEqualToNow_ReturnsNextOccurrence()
    {
        // Arrange - LastRun exactly equal to now
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var now = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(now) // Exactly equal to now
            .WithIsEnabled(true)
            .Build();

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(now);
        result.Value.Day.Should().Be(6); // Next day
    }

    [Fact]
    public void GetNextRunTime_LastRunExactlyEqualToStartUtc_ReturnsNextOccurrence()
    {
        // Arrange - LastRun exactly equal to StartUtc
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(startDate) // Exactly equal to StartUtc
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(startDate);
        result.Value.Day.Should().Be(5); // Should calculate from now
    }

    [Fact]
    public void GetNextRunTime_LastRunExactlyEqualToExpireUtc_ReturnsNull()
    {
        // Arrange - LastRun exactly equal to ExpireUtc
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(expireDate) // Exactly equal to ExpireUtc
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Next run would be day 11, which is >= ExpireUtc (day 10), so should return null
        result.Should().BeNull();
    }

    #endregion

    #region Expiry Boundary Conditions

    [Fact]
    public void GetNextRunTime_ExpireUtcExactlyEqualToNextRun_ReturnsNull()
    {
        // Arrange - ExpireUtc exactly equal to calculated next run
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc); // Next run would be this
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 9, 10, 0, 0, DateTimeKind.Utc); // Same time as StartUtc pattern

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // Next run would be day 10 at 10:00, which is >= ExpireUtc (day 10 at 10:00), so should return null
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_ExpireUtcOneTickAfterNextRun_ReturnsNextRun()
    {
        // Arrange - ExpireUtc one tick after calculated next run
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var nextRun = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = nextRun.AddTicks(1); // One tick after next run
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 9, 10, 0, 0, DateTimeKind.Utc); // Same time pattern

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(nextRun);
        result.Value.Should().BeBefore(expireDate);
    }

    [Fact]
    public void GetNextRunTime_ExpireUtcBeforeStartUtc_ReturnsNull()
    {
        // Arrange - ExpireUtc before StartUtc (invalid, but should handle gracefully)
        var startDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc); // Before StartUtc
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 5, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // StartUtc is in the future, but ExpireUtc is before StartUtc, so trigger is effectively expired
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_ExpireUtcInPast_ReturnsNull()
    {
        // Arrange - ExpireUtc in the past
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc); // After ExpireUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region OneTime Schedule Boundary Conditions

    [Fact]
    public void GetNextRunTime_OneTime_StartUtcExactlyEqualToNow_ReturnsNull()
    {
        // Arrange - OneTime schedule with StartUtc exactly equal to now
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc); // Exactly equal to StartUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns null when now >= StartUtc and LastRun is null (no catch-up)
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTime_StartUtcOneTickAfterNow_ReturnsStartUtc()
    {
        // Arrange - OneTime schedule with StartUtc one tick after now
        var now = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var startDate = now.AddTicks(1); // One tick after now
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns StartUtc when now < StartUtc and LastRun is null
        result.Should().NotBeNull();
        result.Value.Should().Be(startDate);
    }

    [Fact]
    public void GetNextRunTime_OneTime_StartUtcInPastWithLastRunNull_ReturnsNull()
    {
        // Arrange - OneTime schedule with StartUtc in the past and LastRun null
        var startDate = new DateTime(2024, 1, 10, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc); // After StartUtc

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns null when now >= StartUtc and LastRun is null (no catch-up)
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTime_StartUtcInFutureWithLastRunSet_ReturnsNull()
    {
        // Arrange - OneTime schedule with StartUtc in future but LastRun is set
        var startDate = new DateTime(2024, 1, 20, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // CONTRACT: OneTime returns null when LastRun is set (already ran)
        result.Should().BeNull();
    }

    [Fact]
    public void GetNextRunTime_OneTime_ExpireUtcExactlyEqualToStartUtc_ReturnsNull()
    {
        // Arrange - OneTime schedule with ExpireUtc exactly equal to StartUtc
        var startDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.OneTime)
            .WithStartUtc(startDate)
            .WithExpireUtc(startDate) // Exactly equal to StartUtc
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 10, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        // StartUtc is >= ExpireUtc, so should return null
        result.Should().BeNull();
    }

    #endregion

    #region Clock Skew Boundary Conditions

    [Fact]
    public void GetNextRunTime_NowMuchSmallerThanLastRun_CalculatesFromLastRun()
    {
        // Arrange - now much smaller than LastRun (1 year difference)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2025, 1, 5, 10, 0, 0, DateTimeKind.Utc); // 1 year in future
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 9, 0, 0, DateTimeKind.Utc); // 1 year before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day after LastRun
    }

    [Fact]
    public void GetNextRunTime_NowSlightlySmallerThanLastRun_CalculatesFromLastRun()
    {
        // Arrange - now slightly smaller than LastRun (1 second difference)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = lastRun.AddSeconds(-1); // 1 second before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day after LastRun
    }

    [Fact]
    public void GetNextRunTime_NowEqualLastRun_ReturnsNextOccurrence()
    {
        // Arrange - now equal to LastRun
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2024, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = lastRun; // Exactly equal to LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day
    }

    [Fact]
    public void GetNextRunTime_NowAndLastRunBothInFuture_CalculatesFromLastRun()
    {
        // Arrange - now and LastRun both in future relative to StartUtc
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var lastRun = new DateTime(2025, 1, 5, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithLastRun(lastRun)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2025, 1, 1, 9, 0, 0, DateTimeKind.Utc); // Also in future, but before LastRun

        // Act
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeAfter(lastRun);
        result.Value.Day.Should().Be(6); // Next day after LastRun
    }

    #endregion
}

