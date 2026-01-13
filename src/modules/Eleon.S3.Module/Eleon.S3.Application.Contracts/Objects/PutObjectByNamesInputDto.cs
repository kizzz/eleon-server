using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.Application.Contracts.Objects;
public class PutObjectByNamesInputDto
{
    public string BucketName { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public long ContentLength { get; set; }
    public Stream ContentStream { get; set; } = default!;
    public Dictionary<string, string>? Metadata { get; set; }
    public string? Sha256 { get; set; }
}
