using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonS3.Application.Contracts.Multipart;
public class InitiateMultipartOutputDto
{
    public string UploadId { get; set; } = default!; 
}
