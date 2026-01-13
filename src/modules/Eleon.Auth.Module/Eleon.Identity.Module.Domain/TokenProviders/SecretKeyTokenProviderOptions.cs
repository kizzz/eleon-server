using Microsoft.AspNetCore.Identity;
using System;

namespace VPortal.Identity.Module.TokenProviders
{
  public class ApiKeyTokenProviderOptions : DataProtectionTokenProviderOptions
  {
    public static readonly string DefaultName = "ApiKeyTokenProviderOptions";
    public static readonly string DefaultPurpose = "Grants access by secret key";
    public static readonly TimeSpan DefaultLifespan = TimeSpan.FromHours(10);

    public ApiKeyTokenProviderOptions()
    {
      Name = DefaultName;
      TokenLifespan = DefaultLifespan;
    }
  }
}
