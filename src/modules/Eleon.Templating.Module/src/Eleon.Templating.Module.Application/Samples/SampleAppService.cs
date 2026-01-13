using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Eleon.Templating.Module.Samples;

public class SampleAppService : ModuleAppService, ISampleAppService
{
  public Task<SampleDto> GetAsync()
  {
    return Task.FromResult(
        new SampleDto
        {
          Value = 42
        }
    );
  }

  [Authorize]
  public Task<SampleDto> GetAuthorizedAsync()
  {
    return Task.FromResult(
        new SampleDto
        {
          Value = 42
        }
    );
  }
}
