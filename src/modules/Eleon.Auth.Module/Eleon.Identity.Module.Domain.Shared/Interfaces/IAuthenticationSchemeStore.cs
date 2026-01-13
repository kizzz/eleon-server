using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExternalLogin.Module
{
  public interface IAuthenticationSchemeStore
  {
    ValueTask<IEnumerable<AuthenticationScheme>> GetAuthenticationSchemes();
  }
}
