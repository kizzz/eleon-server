using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace VPortal.Identity.Module.TokenProviders
{
  public class ApiKeyTokenProvider<TUser>
      : DataProtectorTokenProvider<TUser>
      where TUser : class
  {
    public ApiKeyTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        ILogger<DataProtectorTokenProvider<TUser>> logger,
        IOptions<ApiKeyTokenProviderOptions> options)
        : base(dataProtectionProvider, options, logger)
    {
    }

    public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
      return Task.FromResult(false);
    }
  }
}
