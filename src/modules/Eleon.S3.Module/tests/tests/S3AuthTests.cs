using Xunit;

namespace EleonS3.Test.Tests;

[Trait("Category", "Integration")]
public class S3AuthTests
{
    public S3AuthTests()
    {
    }

    [Fact(DisplayName = "Invalid credentials should return 403 Forbidden", Skip = "TODO: requires explicit invalid-credential scenario against a live S3 endpoint.")]
    public Task InvalidCredentials_ShouldFail()
    {
        return Task.CompletedTask;
        //var client = S3TestClient.CreateClient(_endpoint, _region, "INVALID_KEY", "INVALID_SECRET");

        //// We expect an AmazonS3Exception
        //var ex = await Assert.ThrowsAsync<AmazonS3Exception>(async () =>
        //{
        //    await client.ListBucketsAsync();
        //});

        //// Verify the server rejected the request
        //Assert.True(
        //    ex.StatusCode == HttpStatusCode.Forbidden || ex.StatusCode == HttpStatusCode.Unauthorized,
        //    $"Expected 403 or 401, got {(int)ex.StatusCode} ({ex.StatusCode})"
        //);
    }
}
