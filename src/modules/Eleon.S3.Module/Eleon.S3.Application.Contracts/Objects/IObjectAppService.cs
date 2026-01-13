using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EleonS3.Application.Contracts.Objects;
public interface IObjectAppService : IApplicationService
{
    public Task<ObjectListDto> GetListFileInfoAsync(string bucket, string? prefix);
    public Task<bool> DeleteAsync(string bucket, string key);
    public Task<ObjectMetadataDto> GetHeadAsync(string bucket, string key);
    public Task SaveAsync(string bucket, string key, byte[] bytes);
    public Task<S3ObjectDto> GetAsync(string bucket, string key, string range);


}
