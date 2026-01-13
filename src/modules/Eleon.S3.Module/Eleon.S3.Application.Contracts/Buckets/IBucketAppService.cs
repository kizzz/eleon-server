using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EleonS3.Application.Contracts.Buckets;
public interface IBucketAppService : IApplicationService
{
    Task<BucketDto> CreateAsync(BucketDto input);
    Task<BucketDto> GetByNameAsync(string name);
}
