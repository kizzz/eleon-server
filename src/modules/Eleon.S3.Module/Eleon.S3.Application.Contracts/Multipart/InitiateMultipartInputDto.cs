using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.Application.Contracts.Multipart;
public class InitiateMultipartInputDto 
{ 
    public string BucketName { get; set; } = default!;
    public string Key { get; set; } = default!; 
}
