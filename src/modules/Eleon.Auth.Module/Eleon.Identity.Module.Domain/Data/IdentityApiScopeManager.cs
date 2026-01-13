using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;

namespace VPortal.Identity.Module.Data
{
  public class IdentityApiScopeManager : ITransientDependency
  {
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IApiScopeRepository _apiScopeRepository;
    private readonly IGuidGenerator _guidGenerator;

    public IdentityApiScopeManager(
        IApiResourceRepository apiResourceRepository,
        IApiScopeRepository apiScopeRepository,
        IGuidGenerator guidGenerator)
    {
      _apiResourceRepository = apiResourceRepository;
      _apiScopeRepository = apiScopeRepository;
      _guidGenerator = guidGenerator;
    }

    public async Task<ApiScope> CreateApiScopeAsync(string name)
    {
      var apiScope = await _apiScopeRepository.FindByNameAsync(name);
      if (apiScope == null)
      {
        apiScope = await _apiScopeRepository.InsertAsync(
            new ApiScope(
                _guidGenerator.Create(),
                name,
                name + " API"
            ),
            autoSave: true
        );
      }

      return apiScope;
    }
  }
}
