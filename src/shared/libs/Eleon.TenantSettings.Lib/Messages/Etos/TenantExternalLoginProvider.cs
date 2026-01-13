using Common.Module.Constants;

namespace TenantSettings.Module.Models
{
  public class TenantExternalLoginProvider
  {
    public ExternalLoginProviderType Type { get; set; }
    public bool Enabled { get; set; }
    public string Data { get; set; }
    public string AdminIdentifier { get; set; }
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
  }
}
