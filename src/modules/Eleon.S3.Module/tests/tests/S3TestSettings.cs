using System;
using Microsoft.Extensions.Configuration;

namespace EleonS3.Test.Tests;

internal sealed class S3TestSettings
{
    public const string EnableIntegrationEnvVar = "ELEON_S3_INTEGRATION";

    public IConfiguration Configuration { get; }
    public string BucketName { get; }
    public string TelemetryBucket { get; }

    private S3TestSettings(IConfiguration configuration, string bucketName, string telemetryBucket)
    {
        Configuration = configuration;
        BucketName = bucketName;
        TelemetryBucket = telemetryBucket;
    }

    public static S3TestSettings Load()
    {
        if (!IsIntegrationEnabled())
        {
            throw new InvalidOperationException($"S3 integration tests disabled. Set {EnableIntegrationEnvVar}=1 to enable.");
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var section = configuration.GetSection("S3");
        var endpoint = section["Endpoint"];
        var region = section["Region"];
        if (string.IsNullOrWhiteSpace(endpoint) && string.IsNullOrWhiteSpace(region))
        {
            throw new InvalidOperationException("S3 test configuration missing Endpoint or Region.");
        }

        var accessKey = section["AccessKey"];
        var secretKey = section["SecretKey"];
        if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("S3 test configuration missing AccessKey or SecretKey.");
        }

        var bucketName = section["BucketNameSftp"] ?? section["BucketName"];
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new InvalidOperationException("S3 test configuration missing BucketNameSftp or BucketName.");
        }

        var telemetryBucket = section["Telemetry"];
        if (string.IsNullOrWhiteSpace(telemetryBucket))
        {
            throw new InvalidOperationException("S3 test configuration missing Telemetry bucket.");
        }

        return new S3TestSettings(configuration, bucketName, telemetryBucket);
    }

    internal static bool IsIntegrationEnabled()
    {
        var value = Environment.GetEnvironmentVariable(EnableIntegrationEnvVar);
        return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
