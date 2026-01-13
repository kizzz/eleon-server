using System;
using NSubstitute;
using Volo.Abp.Timing;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for creating and configuring time-related mocks for testing.
/// </summary>
public static class TimeTestHelpers
{
    /// <summary>
    /// Creates a mock IClock that returns a specific DateTime.
    /// </summary>
    /// <param name="now">The DateTime to return for IClock.Now.</param>
    /// <returns>A mock IClock configured to return the specified DateTime.</returns>
    public static IClock CreateClock(DateTime now)
    {
        var clock = Substitute.For<IClock>();
        clock.Now.Returns(now);
        return clock;
    }

    /// <summary>
    /// Creates a mock IClock that returns DateTime.UtcNow.
    /// </summary>
    /// <returns>A mock IClock configured to return DateTime.UtcNow.</returns>
    public static IClock CreateClock()
    {
        return CreateClock(DateTime.UtcNow);
    }
}

