using System;
using Xunit;

namespace Eleon.McpGateway.Module.Test.TestHelpers;

/// <summary>
/// Custom Fact attribute that automatically skips tests by default.
/// Tests can be enabled by setting the RUN_MANUAL_TESTS environment variable to "1" or "true".
/// 
/// Usage: Replace [Fact] with [ManualTestFact] on test methods that should be skipped by default.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class ManualTestFactAttribute : FactAttribute
{
    private const string RunManualTestsEnvVar = "RUN_MANUAL_TESTS";

    public ManualTestFactAttribute()
    {
        // Check if manual tests should be run
        var shouldSkip = ShouldSkipManualTest();
        
        if (shouldSkip)
        {
            Skip = "Manual test skipped by default. Set RUN_MANUAL_TESTS=1 to enable.";
        }
    }

    private static bool ShouldSkipManualTest()
    {
        // Check environment variable
        var runManualTests = Environment.GetEnvironmentVariable(RunManualTestsEnvVar);
        if (!string.IsNullOrWhiteSpace(runManualTests) && 
            (runManualTests.Equals("1", StringComparison.OrdinalIgnoreCase) || 
             runManualTests.Equals("true", StringComparison.OrdinalIgnoreCase)))
        {
            return false; // Don't skip if explicitly enabled
        }

        // Default: skip manual tests
        return true;
    }
}
