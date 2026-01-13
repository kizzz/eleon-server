using Common.Module.Constants;
using Microsoft.Extensions.Configuration;

namespace VPortal.Core.Infrastructure.Module.IdentityServer
{
  public class IdentitySeedClientDescription
  {
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string RootUrl { get; set; }
    public string Type { get; set; }
    public VportalApplicationType ApplicationType
        => Type.IsNullOrEmpty() ? VportalApplicationType.Undefined : Enum.Parse<VportalApplicationType>(Type);

    public static IdentitySeedClientDescription Parse(IConfigurationSection cfg, string name)
        => cfg.GetSection(name).Get<IdentitySeedClientDescription>();
  }
}
