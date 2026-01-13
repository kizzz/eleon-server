using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EleonS3.HttpApi.Helpers;
using Xunit;

namespace EleonS3.Test.Tests;

public class AwsSigV4ChunkedDecoderTests
{
    [Fact]
    public async Task DecodeAsync_ReturnsPayload()
    {
        var payload = "Hello World";
        var chunked = "b;chunk-signature=000\r\nHello World\r\n0\r\n\r\n";
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(chunked));

        var result = await AwsSigV4ChunkedDecoder.DecodeAsync(stream);

        Assert.Equal(payload, Encoding.UTF8.GetString(result));
    }

    [Fact]
    public async Task DecodeAsync_InvalidHeader_Throws()
    {
        var chunked = "zz;chunk-signature=000\r\nabc\r\n0\r\n\r\n";
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(chunked));

        await Assert.ThrowsAsync<InvalidDataException>(() => AwsSigV4ChunkedDecoder.DecodeAsync(stream));
    }

    [Fact]
    public async Task DecodeAsync_MissingCrlfAfterChunk_Throws()
    {
        var chunked = "3;chunk-signature=000\r\nabc"; // missing CRLF after data
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(chunked));

        await Assert.ThrowsAsync<InvalidDataException>(() => AwsSigV4ChunkedDecoder.DecodeAsync(stream));
    }

    [Fact]
    public async Task DecodeAsync_IgnoresBlankLinesAndTrailers()
    {
        var chunked = "\r\n3;chunk-signature=000\r\nabc\r\n0\r\nx-amz-trailer: value\r\n\r\n";
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(chunked));

        var result = await AwsSigV4ChunkedDecoder.DecodeAsync(stream);

        Assert.Equal("abc", Encoding.UTF8.GetString(result));
    }
}
