
using System.Xml;
using System.Xml.Linq;

namespace EleonCore.Modules.S3.HttpApi.S3.Xml;

public static class S3Xml
{
    private static readonly XNamespace Ns = "http://s3.amazonaws.com/doc/2006-03-01/";

    public static string ListBucketResult(string bucket, string? prefix, IEnumerable<(string Key,long Size, DateTime LastModifiedUtc)> items, bool isTruncated=false, int maxKeys=1000)
    {
        var doc = new XDocument(
            new XElement(Ns + "ListBucketResult",
                new XElement(Ns + "Name", bucket),
                new XElement(Ns + "Prefix", prefix ?? string.Empty),
                new XElement(Ns + "KeyCount", items.Count()),
                new XElement(Ns + "MaxKeys", maxKeys),
                new XElement(Ns + "IsTruncated", isTruncated.ToString().ToLowerInvariant()),
                items.Select(i =>
                    new XElement(Ns + "Contents",
                        new XElement(Ns + "Key", i.Key),
                        new XElement(Ns + "LastModified", i.LastModifiedUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                        new XElement(Ns + "ETag", "\""+ Guid.NewGuid().ToString("N") +"\""),
                        new XElement(Ns + "Size", i.Size),
                        new XElement(Ns + "StorageClass", "STANDARD")
                    )
                )
            )
        );
        var settings = new XmlWriterSettings { OmitXmlDeclaration = false, Indent = false };
        using var sw = new StringWriter();
        using (var xw = XmlWriter.Create(sw, settings)) doc.WriteTo(xw);
        return sw.ToString();
    }

    public static string Error(string code, string message, string resource)
    {
        var doc = new XDocument(
            new XElement("Error",
                new XElement("Code", code),
                new XElement("Message", message),
                new XElement("Resource", resource),
                new XElement("RequestId", Guid.NewGuid().ToString("N"))
            )
        );
        return doc.ToString(SaveOptions.DisableFormatting);
    }
}
