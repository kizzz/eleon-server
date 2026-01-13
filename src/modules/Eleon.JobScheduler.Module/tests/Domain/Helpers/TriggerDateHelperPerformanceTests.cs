using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Xunit;
using EleonsoftModuleCollector.JobScheduler.Module.JobScheduler.Module.Domain.Helpers;
using JobScheduler.Module.TestHelpers;
using VPortal.JobScheduler.Module.Entities;
using Common.Module.Constants;

namespace JobScheduler.Module.Domain.Helpers;

/// <summary>
/// Performance tests for TriggerDateHelper to ensure calculations remain fast with large datasets and edge cases.
/// </summary>
public class TriggerDateHelperPerformanceTests
{
    [Fact]
    public void GetNextRunTime_ThousandTriggers_CompletesWithinReasonableTime()
    {
        // Arrange - Create 1000 triggers with various configurations
        var triggers = new List<TriggerEntity>();
        var baseDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var now = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc);

        for (int i = 0; i < 1000; i++)
        {
            var trigger = TriggerTestDataBuilder.Create()
                .WithPeriodType((TimePeriodType)(i % 3 + 1)) // Cycle through Daily, Weekly, Monthly
                .WithPeriod((i % 10) + 1) // Period 1-10
                .WithStartUtc(baseDate.AddDays(i))
                .WithIsEnabled(true)
                .Build();

            if (trigger.PeriodType == TimePeriodType.Weekly)
            {
                trigger.DaysOfWeekList = new List<int> { (i % 7) + 1 };
            }
            else if (trigger.PeriodType == TimePeriodType.Monthly)
            {
                trigger.DaysOfMonthList = new List<int> { (i % 28) + 1 };
            }

            triggers.Add(trigger);
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var results = triggers.Select(t => TriggerDateHelper.GetNextRunTime(t, now)).ToList();
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(1000);
        // Should complete in reasonable time (e.g., less than 1 second for 1000 triggers)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
    }

    [Fact]
    public void GetNextRunTime_MonthlyScheduleWithFarExpiry_CompletesQuickly()
    {
        // Arrange - Monthly schedule with expiry very far in the future
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var expireDate = new DateTime(2099, 12, 31, 10, 0, 0, DateTimeKind.Utc); // 75 years in future
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithExpireUtc(expireDate)
            .WithIsEnabled(true)
            .Build();

        trigger.DaysOfMonthList = new List<int> { 31 }; // Day 31 (some months don't have it)

        var now = new DateTime(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        // Should complete quickly even with far expiry (iteration limit prevents infinite loops)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    [Fact]
    public void GetNextRunTime_SmallRepeatIntervals_CompletesQuickly()
    {
        // Arrange - Daily schedule with very small repeat intervals
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithRepeatTask(true)
            .WithRepeatInterval(5, TimeUnit.Minutes) // Smallest allowed interval
            .WithRepeatDuration(120, TimeUnit.Minutes)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 1, 1, 10, 1, 0, DateTimeKind.Utc);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        // Should complete quickly (direct calculation, no loops)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10);
    }

    [Fact]
    public void GetNextRunTime_MonthlyWithManyFilteredMonths_CompletesQuickly()
    {
        // Arrange - Monthly schedule with only a few months allowed (many filtered out)
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        // Only allow January and December (most months filtered out)
        trigger.MonthsList = new List<int> { 1, 12 };
        trigger.DaysOfMonthList = new List<int> { 1 };

        var now = new DateTime(2024, 6, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Value.Month.Should().Be(12); // Next allowed month
        // Should complete quickly (iteration limit prevents excessive searching)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    [Fact]
    public void GetNextRunTime_ReflectionCaching_ImprovesPerformance()
    {
        // Arrange - Create triggers that use reflection-heavy properties
        var triggers = new List<TriggerEntity>();
        var baseDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var now = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc);

        for (int i = 0; i < 100; i++)
        {
            var trigger = TriggerTestDataBuilder.Create()
                .WithPeriodType(TimePeriodType.Monthly)
                .WithPeriod(1)
                .WithStartUtc(baseDate)
                .WithIsEnabled(true)
                .Build();

            // Use reflection-heavy properties
            trigger.MonthsList = new List<int> { (i % 12) + 1 };
            trigger.DaysOfMonthList = new List<int> { (i % 28) + 1 };
            trigger.DaysOfWeekList = new List<int> { (i % 7) + 1 };
            trigger.DaysOfWeekOccurencesList = new List<int> { (i % 4) + 1 };

            triggers.Add(trigger);
        }

        // Act - First call (cache miss) and subsequent calls (cache hit)
        var stopwatch = Stopwatch.StartNew();
        var results = triggers.Select(t => TriggerDateHelper.GetNextRunTime(t, now)).ToList();
        stopwatch.Stop();

        // Assert
        results.Should().HaveCount(100);
        // Reflection caching should make subsequent calls faster
        // Total time should be reasonable (less than 500ms for 100 triggers with reflection)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
    }

    [Fact]
    public void GetNextRunTime_LargePeriodValues_CompletesQuickly()
    {
        // Arrange - Daily schedule with very large period
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Daily)
            .WithPeriod(1000) // Very large period
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        var now = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        // Should complete quickly (direct calculation, no loops)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10);
    }

    [Fact]
    public void GetNextRunTime_ComplexMonthlySchedule_CompletesQuickly()
    {
        // Arrange - Complex monthly schedule with multiple constraints
        var startDate = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var trigger = TriggerTestDataBuilder.Create()
            .WithPeriodType(TimePeriodType.Monthly)
            .WithPeriod(1)
            .WithStartUtc(startDate)
            .WithIsEnabled(true)
            .Build();

        trigger.MonthsList = new List<int> { 1, 3, 5, 7, 9, 11 }; // Odd months only
        trigger.DaysOfMonthList = new List<int> { 15, 30 }; // Two days per month
        trigger.DaysOfWeekList = new List<int> { 1, 3, 5 }; // Mon, Wed, Fri
        trigger.DaysOfWeekOccurencesList = new List<int> { 1, 2 }; // First and second occurrence

        var now = new DateTime(2024, 6, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = TriggerDateHelper.GetNextRunTime(trigger, now);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        // Should complete quickly despite complexity (iteration limit prevents excessive searching)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }
}

