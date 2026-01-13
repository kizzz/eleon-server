using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using EleonCore.Modules.S3.HttpApi.S3.Xml;
using Xunit;

namespace EleonS3.Test.Tests;

public class S3XmlTests
{
    [Fact]
    public void ListBucketResult_ContainsExpectedValues()
    {
        var items = new List<(string Key, long Size, DateTime LastModifiedUtc)>
        {
            ("a.txt", 10, new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)),
            ("b.txt", 20, new DateTime(2024, 01, 02, 0, 0, 0, DateTimeKind.Utc))
        };

        var xml = S3Xml.ListBucketResult("bucket", "prefix", items, isTruncated: true, maxKeys: 500);
        var doc = XDocument.Parse(xml);
        var ns = XNamespace.Get("http://s3.amazonaws.com/doc/2006-03-01/");

        Assert.Equal("bucket", doc.Root?.Element(ns + "Name")?.Value);
        Assert.Equal("prefix", doc.Root?.Element(ns + "Prefix")?.Value);
        Assert.Equal("2", doc.Root?.Element(ns + "KeyCount")?.Value);
        Assert.Equal("500", doc.Root?.Element(ns + "MaxKeys")?.Value);
        Assert.Equal("true", doc.Root?.Element(ns + "IsTruncated")?.Value);

        var keys = doc.Descendants(ns + "Key").Select(x => x.Value).ToList();
        Assert.Contains("a.txt", keys);
        Assert.Contains("b.txt", keys);
    }

    [Fact]
    public void Error_ReturnsXmlWithFields()
    {
        var xml = S3Xml.Error("AccessDenied", "Denied", "bucket/key");
        var doc = XDocument.Parse(xml);

        Assert.Equal("AccessDenied", doc.Root?.Element("Code")?.Value);
        Assert.Equal("Denied", doc.Root?.Element("Message")?.Value);
        Assert.Equal("bucket/key", doc.Root?.Element("Resource")?.Value);
        Assert.False(string.IsNullOrWhiteSpace(doc.Root?.Element("RequestId")?.Value));
    }
}
