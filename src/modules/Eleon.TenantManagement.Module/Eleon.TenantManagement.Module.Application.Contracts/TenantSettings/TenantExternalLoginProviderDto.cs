using Common.Module.Constants;

namespace TenantSettings.Module.Cache
{
  public class TenantExternalLoginProviderDto
  {
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public ExternalLoginProviderType Type { get; set; }
    public bool Enabled { get; set; }
    public string AdminIdentifier { get; set; }
  }
}
