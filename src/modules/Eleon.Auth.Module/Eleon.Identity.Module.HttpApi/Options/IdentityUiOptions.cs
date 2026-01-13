using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace VPortal.Options
{
  public class IdentityUiOptions
  {
    public List<string> VehicleDriverCredentialsClients { get; set; } = new();

    public IdentityUiOptions PreConfigure(IConfiguration configuration)
    {
      var identityUiOptions = configuration
          .GetSection("IdentityUI")
          .Get<IdentityUiOptions>();

      if (identityUiOptions != null)
      {
        VehicleDriverCredentialsClients = identityUiOptions.VehicleDriverCredentialsClients;
      }

      return this;
    }
  }
}
