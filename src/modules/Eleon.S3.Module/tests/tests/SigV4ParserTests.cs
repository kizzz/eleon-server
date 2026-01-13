using System;
using EleonCore.Modules.S3.HttpApi.S3.SigV4;
using Xunit;

namespace EleonS3.Test.Tests;

public class SigV4ParserTests
{
    [Fact]
    public void Parse_ValidHeader_ReturnsParts()
    {
        var header = "AWS4-HMAC-SHA256 Credential=AKIDEXAMPLE/20240101/us-east-1/s3/aws4_request, SignedHeaders=host;x-amz-date, Signature=" + new string('a', 64);

        var parts = SigV4Parser.Parse(header);

        Assert.Equal("AKIDEXAMPLE", parts.AccessKeyId);
        Assert.Equal("20240101", parts.CredentialScopeDate);
        Assert.Equal("us-east-1", parts.CredentialScopeRegion);
        Assert.Equal("s3", parts.CredentialScopeService);
        Assert.Equal("host;x-amz-date", parts.SignedHeadersCsv);
        Assert.Equal(new string('a', 64), parts.SignatureHex);
    }

    [Fact]
    public void Parse_InvalidHeader_Throws()
    {
        var header = "invalid-header";

        Assert.Throws<InvalidOperationException>(() => SigV4Parser.Parse(header));
    }

    [Fact]
    public void Parse_UppercaseSignature_Lowercases()
    {
        var signature = new string('A', 64);
        var header = $"AWS4-HMAC-SHA256 Credential=AKID/20240101/us-east-1/s3/aws4_request, SignedHeaders=host, Signature={signature}";

        var parts = SigV4Parser.Parse(header);

        Assert.Equal(new string('a', 64), parts.SignatureHex);
    }
}
