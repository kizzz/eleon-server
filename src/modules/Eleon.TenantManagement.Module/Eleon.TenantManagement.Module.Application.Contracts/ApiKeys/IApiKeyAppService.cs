using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys
{
  public interface IApiKeyAppService : IApplicationService
  {
    Task<IdentityApiKeyDto> GetByIdAsync(Guid id);
    Task<List<IdentityApiKeyDto>> GetApiKeysAsync(ApiKeyRequestDto request);
    Task RemoveApiKeyAsync(Guid id);
    Task<IdentityApiKeyDto> AddSdkKeyAsync(string name);
    Task<IdentityApiKeyDto> AddIdentityApiKeyAsync(CreateApiKeyDto request);
    Task<IdentityApiKeyDto> UpdateAsync(UpdateApiKeyDto request);
  }
}
