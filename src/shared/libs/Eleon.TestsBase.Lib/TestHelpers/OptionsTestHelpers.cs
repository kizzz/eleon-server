using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Volo.Abp.Options;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Test implementation of AbpDynamicOptionsManager for use in unit tests.
/// </summary>
/// <typeparam name="TOptions">The options type.</typeparam>
public sealed class TestAbpDynamicOptionsManager<TOptions> : AbpDynamicOptionsManager<TOptions>
    where TOptions : class, new()
{
    private TOptions _value;

    /// <summary>
    /// Initializes a new instance of TestAbpDynamicOptionsManager.
    /// </summary>
    public TestAbpDynamicOptionsManager() : base(
        Substitute.For<IOptionsFactory<TOptions>>())
    {
        _value = new TOptions();
    }

    /// <summary>
    /// Overrides the options asynchronously.
    /// </summary>
    protected override Task OverrideOptionsAsync(string name, TOptions options)
    {
        _value = options;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the current options value.
    /// </summary>
    public new TOptions Value => _value;
}

/// <summary>
/// Helpers for creating test options managers.
/// </summary>
public static class OptionsTestHelpers
{
    /// <summary>
    /// Creates a test AbpDynamicOptionsManager instance for the specified options type.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    /// <returns>A test AbpDynamicOptionsManager instance.</returns>
    public static TestAbpDynamicOptionsManager<TOptions> CreateTestAbpDynamicOptionsManager<TOptions>()
        where TOptions : class, new()
    {
        return new TestAbpDynamicOptionsManager<TOptions>();
    }
}

