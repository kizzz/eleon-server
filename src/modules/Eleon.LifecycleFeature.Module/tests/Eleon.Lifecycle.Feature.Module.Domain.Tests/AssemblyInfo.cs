using Xunit;

// Disable parallel test execution to prevent DI container disposal issues
[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]

