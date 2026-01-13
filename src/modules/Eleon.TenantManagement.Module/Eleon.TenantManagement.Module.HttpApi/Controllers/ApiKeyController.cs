using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using ModuleCollector.Identity.Module.Identity.Module.Application.Contracts.ApiKeys;
using Volo.Abp;
using VPortal.TenantManagement.Module;


namespace Core.Infrastructure.Module.Controllers
{
  [Area(TenantManagementRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = TenantManagementRemoteServiceConsts.RemoteServiceName)]
  [Route("api/Identity/ApiKeys/")]
  public class ApiKeyController : TenantManagementController, IApiKeyAppService
  {
    private readonly IVportalLogger<ApiKeyController> logger;
    private readonly IApiKeyAppService apiKeyAppService;

    public ApiKeyController(
        IVportalLogger<ApiKeyController> logger,
        IApiKeyAppService apiKeyAppService)
    {
      this.logger = logger;
      this.apiKeyAppService = apiKeyAppService;
    }

    [HttpPost("AddSdkKey")]
    public async Task<IdentityApiKeyDto> AddSdkKeyAsync(string name)
    {

      var response = await apiKeyAppService.AddSdkKeyAsync(name);


      return response;
    }

    [HttpPost("AddIdentityApiKey")]
    public async Task<IdentityApiKeyDto> AddIdentityApiKeyAsync(CreateApiKeyDto request)
    {

      var response = await apiKeyAppService.AddIdentityApiKeyAsync(request);


      return response;
    }

    [HttpPost("GetApiKeys")]
    public async Task<List<IdentityApiKeyDto>> GetApiKeysAsync(ApiKeyRequestDto request)
    {

      var response = await apiKeyAppService.GetApiKeysAsync(request);


      return response;
    }

    [HttpPost("RemoveApiKey")]
    public async Task RemoveApiKeyAsync(Guid id)
    {

      await apiKeyAppService.RemoveApiKeyAsync(id);

    }

    [HttpPost("Update")]
    public async Task<IdentityApiKeyDto> UpdateAsync(UpdateApiKeyDto request)
    {

      var response = await apiKeyAppService.UpdateAsync(request);


      return response;
    }

    [HttpGet("GetById")]
    public async Task<IdentityApiKeyDto> GetByIdAsync(Guid id)
    {

      var response = await apiKeyAppService.GetByIdAsync(id);


      return response;
    }
  }
}
