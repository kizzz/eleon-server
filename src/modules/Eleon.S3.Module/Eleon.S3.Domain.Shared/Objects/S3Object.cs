using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace EleonS3.Domain.Shared.Objects;
public class S3Object
{
    [NotMapped]
    public bool IsFullObject { get; set; }
    [NotMapped]
    public string ETag { get; set; }
    [NotMapped]
    public long End { get; set; }
    [NotMapped]
    public long Start { get; set; }
    [NotMapped]
    public byte[] Content { get; set; }
    [NotMapped]
    public byte[] Slice { get; set; }
    [NotMapped]
    public int ContentLength => Content?.Length ?? 0;
}
