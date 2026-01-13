using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EleonS3.Application.Contracts.Multipart;
public interface IMultipartAppService : IApplicationService
{
    Task<InitiateMultipartOutputDto> InitiateAsync(InitiateMultipartInputDto input);
    Task UploadPartAsync(UploadPartDto input);
    Task CompleteAsync(string bucketName, string key, string uploadId, IEnumerable<int> orderedPartNumbers);
    Task AbortAsync(string bucketName, string key, string uploadId);
}
