using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using EleonS3.Application.Contracts.Objects;
using EleonS3.HttpApi.Controllers;
using Logging.Module;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using Xunit;

namespace EleonS3.Test.Tests;

public class S3ObjectControllerTests
{
    [Fact]
    public async Task InitiateMultipartUpload_ReturnsBadRequest()
    {
        var controller = BuildController();

        var result = await controller.InitiateMultipartUpload("bucket", "key", "uploads");

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Put_WithMultipartParams_ReturnsBadRequest()
    {
        var controller = BuildController();
        controller.ControllerContext.HttpContext.Request.Body = new MemoryStream();

        var result = await controller.Put("bucket", "key", partNumber: 1, uploadId: null);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Put_MissingBody_ReturnsBadRequest()
    {
        var controller = BuildController();
        controller.ControllerContext.HttpContext.Request.Body = null;

        var result = await controller.Put("bucket", "key", null, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("MissingRequestBody", badRequest.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task Put_WithChunkedBody_DecodesAndSaves()
    {
        var appService = Substitute.For<IObjectAppService>();
        var controller = BuildController(appService: appService);

        var chunked = "b;chunk-signature=000\r\nHello World\r\n0\r\n\r\n";
        controller.ControllerContext.HttpContext.Request.Headers["x-amz-decoded-content-length"] = "11";
        controller.ControllerContext.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(chunked));

        var result = await controller.Put("bucket", "key", null, null);

        Assert.IsType<OkObjectResult>(result);
        await appService.Received().SaveAsync("bucket", "key", Arg.Any<byte[]>());
    }

    [Fact]
    public async Task Put_WhenServiceThrows_ReturnsBadRequest()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.SaveAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>())
            .Returns<Task>(_ => throw new Exception("boom"));
        var controller = BuildController(appService: appService);
        controller.ControllerContext.HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("payload"));

        var result = await controller.Put("bucket", "key", null, null);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Get_WithRange_ReturnsPartial()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetAsync("bucket", "key", Arg.Any<string>())
            .Returns(Task.FromResult(new S3ObjectDto
            {
                IsFullObject = false,
                Start = 0,
                End = 3,
                Content = new byte[] { 1, 2, 3, 4 },
                Slice = new byte[] { 1, 2, 3, 4 }
            }));
        var controller = BuildController(appService: appService);
        controller.ControllerContext.HttpContext.Request.Headers[HeaderNames.Range] = "bytes=0-3";

        var result = await controller.Get("bucket", "key");

        Assert.Equal(StatusCodes.Status206PartialContent, controller.ControllerContext.HttpContext.Response.StatusCode);
        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal(4, file.FileContents.Length);
    }

    [Fact]
    public async Task Get_Full_ReturnsFile()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetAsync("bucket", "key", Arg.Any<string>())
            .Returns(Task.FromResult(new S3ObjectDto
            {
                IsFullObject = true,
                Content = new byte[] { 1, 2 }
            }));
        var controller = BuildController(appService: appService);

        var result = await controller.Get("bucket", "key");

        var file = Assert.IsType<FileContentResult>(result);
        Assert.Equal(2, file.FileContents.Length);
    }

    [Fact]
    public async Task Get_WhenServiceThrows_ReturnsBadRequest()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns<Task<S3ObjectDto>>(_ => throw new Exception("boom"));
        var controller = BuildController(appService: appService);

        var result = await controller.Get("bucket", "key");

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Head_WhenMissing_ReturnsNotFound()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetHeadAsync("bucket", "key").Returns(Task.FromResult<ObjectMetadataDto>(null));
        var controller = BuildController(appService: appService);

        var result = await controller.Head("bucket", "key");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Head_WhenFound_SetsHeaders()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetHeadAsync("bucket", "key").Returns(Task.FromResult(new ObjectMetadataDto
        {
            ContentLength = 10,
            ETag = "etag"
        }));
        var controller = BuildController(appService: appService);

        var result = await controller.Head("bucket", "key");

        Assert.IsType<OkResult>(result);
        Assert.Equal(10, controller.ControllerContext.HttpContext.Response.ContentLength);
        Assert.Equal("etag", controller.ControllerContext.HttpContext.Response.Headers.ETag);
    }

    [Fact]
    public async Task Delete_WithUploadId_ReturnsBadRequest()
    {
        var controller = BuildController();

        var result = await controller.Delete("bucket", "key", "uploadId");

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_WhenDeleted_ReturnsNoContent()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.DeleteAsync("bucket", "key").Returns(Task.FromResult(true));
        var controller = BuildController(appService: appService);

        var result = await controller.Delete("bucket", "key", null);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.DeleteAsync("bucket", "key").Returns(Task.FromResult(false));
        var controller = BuildController(appService: appService);

        var result = await controller.Delete("bucket", "key", null);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ListObjectsV2_InvalidListType_ReturnsBadRequest()
    {
        var controller = BuildController();

        var result = await controller.ListObjectsV2("bucket", listType: 1, prefix: null);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task ListObjectsV2_ReturnsXml()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetListFileInfoAsync("bucket", Arg.Any<string?>())
            .Returns(Task.FromResult(new ObjectListDto { Xml = "<ListBucketResult/>" }));
        var controller = BuildController(appService: appService);

        var result = await controller.ListObjectsV2("bucket", listType: 2, prefix: "p");

        var content = Assert.IsType<ContentResult>(result);
        Assert.StartsWith("application/xml", content.ContentType);
        Assert.Contains("ListBucketResult", content.Content);
    }

    [Fact]
    public async Task ListObjectsV2_WhenServiceThrows_ReturnsBadRequest()
    {
        var appService = Substitute.For<IObjectAppService>();
        appService.GetListFileInfoAsync(Arg.Any<string>(), Arg.Any<string?>())
            .Returns<Task<ObjectListDto>>(_ => throw new Exception("boom"));
        var controller = BuildController(appService: appService);

        var result = await controller.ListObjectsV2("bucket", listType: 2, prefix: null);

        Assert.IsType<BadRequestResult>(result);
    }

    private static S3ObjectController BuildController(IObjectAppService? appService = null)
    {
        appService ??= Substitute.For<IObjectAppService>();
        var logger = TestMockHelpers.CreateMockLogger<S3ObjectController>();
        var controller = new S3ObjectController(appService, logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}
