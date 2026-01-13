using System;
using System.Collections.Generic;

namespace TenantSettings.Module.Cache
{
  public class SetTenantProviderSettingsRequestDto
  {
    public Guid? TenantId { get; set; }
    public List<TenantExternalLoginProviderDto> Providers { get; set; }
  }
}
