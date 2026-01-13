using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Xunit;

namespace EleonS3.Test.Tests;

public class S3BucketTests
{
    private readonly IAmazonS3 _s3;

    public S3BucketTests()
    {
        if (!File.Exists("appsettings.json"))
        {
            _s3 = null!;
            return;
        }

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        _s3 = S3TestClient.Create(config);
    }

    //[Fact]
    //public async Task CanCreateAndDeleteBucket()
    //{
    //    var bucketName = $"testbucket-{Guid.NewGuid():N}".ToLower();

    //    await _s3.PutBucketAsync(bucketName);
    //    var buckets = await _s3.ListBucketsAsync();
    //    var exists = buckets.Buckets.Any(b => b.BucketName == bucketName);

    //    Assert.True(exists, $"Expected bucket '{bucketName}' to exist after creation.");

    //    await _s3.DeleteBucketAsync(bucketName);
    //    var bucketsAfterDelete = await _s3.ListBucketsAsync();
    //    var stillExists = bucketsAfterDelete.Buckets.Any(b => b.BucketName == bucketName);

    //    Assert.False(stillExists, $"Expected bucket '{bucketName}' to be deleted.");
    //}
}
