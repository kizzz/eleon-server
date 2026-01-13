using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.Application.Contracts.Objects;
public class S3ObjectDto
{   
    public bool IsFullObject { get; set; }
    
    public string Etag { get; set; }
    
    public long End { get; set; }
    
    public long Start { get; set; }
    
    public byte[] Content { get; set; }
    
    public byte[] Slice { get; set; }
}
