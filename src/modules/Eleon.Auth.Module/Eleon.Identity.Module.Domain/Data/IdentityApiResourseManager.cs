using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.IdentityServer.ApiResources;

namespace VPortal.Identity.Module.Data
{
  public class IdentityApiResourseManager : ITransientDependency
  {
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IGuidGenerator _guidGenerator;

    public IdentityApiResourseManager(
        IApiResourceRepository apiResourceRepository,
        IGuidGenerator guidGenerator)
    {
      _apiResourceRepository = apiResourceRepository;
      _guidGenerator = guidGenerator;
    }

    public async Task<ApiResource> CreateApiResourceAsync(string name, IEnumerable<string> claims)
    {
      var apiResource = await _apiResourceRepository.FindByNameAsync(name);
      if (apiResource == null)
      {
        apiResource = await _apiResourceRepository.InsertAsync(
            new ApiResource(
                _guidGenerator.Create(),
                name,
                name + " API"
            ),
            autoSave: true
        );
      }

      foreach (var claim in claims)
      {
        if (apiResource.FindClaim(claim) == null)
        {
          apiResource.AddUserClaim(claim);
        }
      }

      return await _apiResourceRepository.UpdateAsync(apiResource);
    }
  }
}
