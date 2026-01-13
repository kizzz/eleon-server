using Amazon.S3;
using Amazon.S3.Model;
using Xunit;

namespace EleonS3.Test.Tests;

[Trait("Category", "Integration")]
public class S3BasicTests
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;
    private readonly string _telemetryBucket;

    public S3BasicTests()
    {
        var settings = S3TestSettings.Load();
        _s3 = S3TestClient.Create(settings.Configuration);
        _bucket = settings.BucketName;
        _telemetryBucket = settings.TelemetryBucket;
    }

    [S3IntegrationFact]
    public async Task CanPutAndGetObject()
    {
        await CanPutAndGetObjectHelper(_bucket);
        await CanPutAndGetObjectHelper(_telemetryBucket);
    }
    private string CurrentTimestamp() => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000Z").Replace(' ', '-').Replace('.', '-').Replace(':', '-');
    private async Task CanPutAndGetObjectHelper(string bucketName)
    {
        var now = CurrentTimestamp();
        var key = $"{now}/putandgate/hello.txt";
        var content = "Hello from Eleonsoft S3 mock";

        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            ContentBody = content
        });

        var get = await _s3.GetObjectAsync(bucketName, key);
        using var reader = new StreamReader(get.ResponseStream);
        var result = await reader.ReadToEndAsync();

        Assert.Equal(content, result);
    }

    [S3IntegrationFact]
    public async Task CanListObjects()
    {
        var response = await _s3.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = _bucket,
            Prefix = "./"
        });

        Assert.NotNull(response);
        Assert.True(response.S3Objects.Count > 0, "Expected at least one object in 'test/' prefix");
    }

    [S3IntegrationFact]
    public async Task CanDeleteObject()
    {
        var now = CurrentTimestamp();
        var key = $"{now}/delete/delete_me.txt";
        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            ContentBody = "to be deleted"
        });

        await _s3.DeleteObjectAsync(_bucket, key);

        var list = await _s3.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = _bucket,
            Prefix = "test/delete"
        });

        Assert.Null(list.S3Objects);
    }

    [S3IntegrationFact(DisplayName = "Can perform HEAD request for existing object")]
    public async Task CanHeadObject()
    {
        var now = CurrentTimestamp();
        var key = $"{now}/head/headcheck.txt";
        var content = "This is a head test.";

        // Upload the object first
        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            ContentBody = content
        });

        // Perform HEAD request
        var response = await _s3.GetObjectMetadataAsync(new GetObjectMetadataRequest
        {
            BucketName = _bucket,
            Key = key
        });

        Assert.NotNull(response);
        Assert.Equal(content.Length, response.ContentLength);
        Assert.False(string.IsNullOrEmpty(response.ETag));
    }

    [S3IntegrationFact(DisplayName = "HEAD request returns 404 for missing object")]
    public async Task HeadObject_NotFound_Returns404()
    {
        var now = CurrentTimestamp();
        var key = $"{now}/notfound/nonexistent.txt";

        var ex = await Assert.ThrowsAsync<AmazonS3Exception>(async () =>
        {
            await _s3.GetObjectMetadataAsync(new GetObjectMetadataRequest
            {
                BucketName = _bucket,
                Key = key
            });
        });

        Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
    }
}
