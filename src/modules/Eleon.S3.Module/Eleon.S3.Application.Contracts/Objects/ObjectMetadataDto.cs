using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.Application.Contracts.Objects;
public class ObjectMetadataDto
{
    public int ContentLength { get; set; }
    public string ETag { get; set; }
}
