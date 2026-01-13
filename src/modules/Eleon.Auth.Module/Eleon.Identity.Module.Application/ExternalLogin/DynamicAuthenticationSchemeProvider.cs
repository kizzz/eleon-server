using ExternalLogin.Module;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Identity.Module.AuthenticationSchemes
{
  public class DynamicAuthenticationSchemeProvider : AuthenticationSchemeProvider, ISingletonDependency
  {
    private readonly IAuthenticationSchemeStore store;

    public DynamicAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options, IAuthenticationSchemeStore store)
        : base(options)
    {
      this.store = store;
    }

    protected DynamicAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options, IDictionary<string, AuthenticationScheme> schemes)
        : base(options, schemes)
    {
    }

    public override async Task<AuthenticationScheme> GetSchemeAsync(string name)
    {
      var dynamicSchemes = await store.GetAuthenticationSchemes();
      var dynamicScheme = dynamicSchemes.FirstOrDefault(x => x.Name == name);
      if (dynamicScheme != null)
      {
        return dynamicScheme;
      }

      return await base.GetSchemeAsync(name);
    }

    public override async Task<IEnumerable<AuthenticationScheme>> GetAllSchemesAsync()
    {
      var dynamicSchemes = await store.GetAuthenticationSchemes();
      var staticSchemes = await base.GetAllSchemesAsync();
      return dynamicSchemes.Concat(staticSchemes);
    }

    public override async Task<IEnumerable<AuthenticationScheme>> GetRequestHandlerSchemesAsync()
    {
      var dynamicSchemes = await store.GetAuthenticationSchemes();
      var staticSchemes = await base.GetRequestHandlerSchemesAsync();
      return dynamicSchemes.Concat(staticSchemes);
    }
  }
}
