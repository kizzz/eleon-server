using System;
using Xunit;

namespace EleonS3.Test.Tests;

[AttributeUsage(AttributeTargets.Method)]
public sealed class S3IntegrationFactAttribute : FactAttribute
{
    public S3IntegrationFactAttribute()
    {
        if (!S3TestSettings.IsIntegrationEnabled())
        {
            Skip = $"S3 integration tests disabled. Set {S3TestSettings.EnableIntegrationEnvVar}=1 to enable.";
        }
    }
}
