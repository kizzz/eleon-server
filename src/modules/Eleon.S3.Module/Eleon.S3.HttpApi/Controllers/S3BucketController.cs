using EleonCore.Modules.S3.HttpApi.S3.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;
using Volo.Abp.AspNetCore.Mvc;

namespace EleonS3.HttpApi.Controllers;

[ApiController]
[Route("s3")]
public class S3BucketController : AbpController
{
    [HttpGet]
    public IActionResult ListAllBuckets()
    {
        throw new NotImplementedException("Eleons3 list all buckets not implemented");
    //    // Example static bucket list for now
    //    var buckets = new[]
    //    {
    //    new { Name = "grafana", CreationDate = DateTime.UtcNow }
    //};

    //    var ns = "http://s3.amazonaws.com/doc/2006-03-01/";
    //    var doc = new XDocument(
    //        new XElement(XName.Get("ListAllMyBucketsResult", ns),
    //            new XElement(XName.Get("Owner", ns),
    //                new XElement(XName.Get("ID", ns), "mock-owner"),
    //                new XElement(XName.Get("DisplayName", ns), "eleons3")
    //            ),
    //            new XElement(XName.Get("Buckets", ns),
    //                buckets.Select(b =>
    //                    new XElement(XName.Get("Bucket", ns),
    //                        new XElement(XName.Get("Name", ns), b.Name),
    //                        new XElement(XName.Get("CreationDate", ns), b.CreationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
    //                    )
    //                )
    //            )
    //        )
    //    );

    //    return Content(doc.ToString(SaveOptions.DisableFormatting), "application/xml");
    }
}
