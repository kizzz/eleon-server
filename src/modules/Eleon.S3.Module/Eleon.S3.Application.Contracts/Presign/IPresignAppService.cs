using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EleonS3.Application.Contracts.Presign;

public interface IPresignAppService : IApplicationService
{
    Task<string> CreateUrlAsync(string method, string path, string? query, TimeSpan ttl, string? bodySha256 = null);
}
