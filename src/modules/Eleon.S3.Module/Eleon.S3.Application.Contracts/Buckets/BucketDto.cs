using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.Application.Contracts.Buckets;
public class BucketDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ProviderName { get; set; } = "Minio";
    public string? ProviderConfigJson { get; set; }
    public int Visibility { get; set; }
    public bool VersioningEnabled { get; set; }
    public string? PolicyJson { get; set; }
}
