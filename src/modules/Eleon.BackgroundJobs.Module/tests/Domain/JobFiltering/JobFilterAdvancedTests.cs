using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;
using BackgroundJobs.Module.JobFiltering;
using BackgroundJobs.Module.TestHelpers;
using VPortal.BackgroundJobs.Module.Entities;

namespace BackgroundJobs.Module.Domain.JobFiltering;

/// <summary>
/// Advanced JobFilter scenario tests
/// </summary>
public class JobFilterAdvancedTests
{
    private IConfiguration CreateConfiguration(Dictionary<string, string> configValues)
    {
        var builder = new ConfigurationBuilder();
        if (configValues != null)
        {
            builder.AddInMemoryCollection(configValues);
        }
        return builder.Build();
    }

    [Fact]
    public void Filter_MultiTenantWhitelist_FiltersCorrectly()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:Tenants:0", TestConstants.TenantIds.Tenant1.ToString() },
            { "JobOptions:Whitelist:Tenants:1", TestConstants.TenantIds.Tenant2.ToString() }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant2)
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(Guid.NewGuid())
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert
        filtered.Should().HaveCount(2);
        filtered.Should().OnlyContain(j => j.TenantId == TestConstants.TenantIds.Tenant1 || 
                                           j.TenantId == TestConstants.TenantIds.Tenant2);
    }

    [Fact]
    public void Filter_HostTenantInWhitelist_FiltersCorrectly()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:Tenants:0", "host" }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(null) // Host tenant
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert
        filtered.Should().HaveCount(1);
        filtered[0].TenantId.Should().BeNull();
    }

    [Fact]
    public void Filter_EnvironmentIdFiltering_FiltersCorrectly()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:EnvironmentIds:0", "env1" },
            { "JobOptions:Whitelist:EnvironmentIds:1", "env2" }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithEnvironmentId("env1")
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithEnvironmentId("env2")
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithEnvironmentId("env3")
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert
        filtered.Should().HaveCount(2);
        filtered.Should().OnlyContain(j => j.EnvironmentId == "env1" || j.EnvironmentId == "env2");
    }

    [Fact]
    public void Filter_CombinedTenantAndEnvironment_FiltersCorrectly()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:Tenants:0", TestConstants.TenantIds.Tenant1.ToString() },
            { "JobOptions:Whitelist:EnvironmentIds:0", "env1" }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .WithEnvironmentId("env1")
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .WithEnvironmentId("env2")
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant2)
                .WithEnvironmentId("env1")
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert
        filtered.Should().HaveCount(1);
        filtered[0].TenantId.Should().Be(TestConstants.TenantIds.Tenant1);
        filtered[0].EnvironmentId.Should().Be("env1");
    }

    [Fact]
    public void Filter_EmptyFilterResults_ReturnsEmpty()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:Tenants:0", Guid.NewGuid().ToString() }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert
        filtered.Should().BeEmpty();
    }

    [Fact]
    public void Filter_WhitelistDisabled_ReturnsAllJobs()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "false" }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .Build(),
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant2)
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert
        filtered.Should().HaveCount(2);
    }

    [Fact]
    public void Filter_NullConfiguration_HandlesGracefully()
    {
        // Arrange
        var config = CreateConfiguration(null);
        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create().Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert - Should not throw
        filtered.Should().HaveCount(1);
    }

    [Fact]
    public void Filter_CaseSensitiveTenantIds_HandlesCorrectly()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:Tenants:0", TestConstants.TenantIds.Tenant1.ToString().ToUpper() }
        });

        var filter = new JobFilter(config);

        var jobs = new List<BackgroundJobEntity>
        {
            BackgroundJobTestDataBuilder.Create()
                .WithTenantId(TestConstants.TenantIds.Tenant1)
                .Build()
        };

        // Act
        var filtered = filter.Filter(jobs).ToList();

        // Assert - GUID parsing should handle case
        filtered.Should().HaveCount(1);
    }

    [Fact]
    public void Filter_LargeJobSet_PerformsEfficiently()
    {
        // Arrange
        var config = CreateConfiguration(new Dictionary<string, string>
        {
            { "JobOptions:Whitelist:Enabled", "true" },
            { "JobOptions:Whitelist:Tenants:0", TestConstants.TenantIds.Tenant1.ToString() }
        });

        var filter = new JobFilter(config);

        var jobs = Enumerable.Range(0, 1000).Select(i =>
            BackgroundJobTestDataBuilder.Create()
                .WithId(Guid.NewGuid())
                .WithTenantId(i % 2 == 0 ? TestConstants.TenantIds.Tenant1 : TestConstants.TenantIds.Tenant2)
                .Build()
        ).ToList();

        // Act
        var startTime = DateTime.UtcNow;
        var filtered = filter.Filter(jobs).ToList();
        var duration = DateTime.UtcNow - startTime;

        // Assert
        filtered.Should().HaveCount(500);
        duration.Should().BeLessThan(TimeSpan.FromSeconds(1)); // Should be fast
    }
}

