using System;
using EleonS3.HttpApi.Controllers;
using Xunit;

namespace EleonS3.Test.Tests;

public class S3BucketControllerTests
{
    [Fact]
    public void ListAllBuckets_ThrowsNotImplemented()
    {
        var controller = new S3BucketController();

        Assert.Throws<NotImplementedException>(() => controller.ListAllBuckets());
    }
}
