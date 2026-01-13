using ExternalLogin.Module;
using System.Threading.Tasks;
using Volo.Abp.Clients;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace VPortal.Identity.Module.ExternalProviders
{
  public class ExternalLoginSecurityLogManager : IExternalLoginSecurityLogManager, ITransientDependency
  {
    private readonly IdentitySecurityLogManager identitySecurityLogManager;
    private readonly CurrentClient currentClient;

    public ExternalLoginSecurityLogManager(IdentitySecurityLogManager identitySecurityLogManager, CurrentClient currentClient)
    {
      this.identitySecurityLogManager = identitySecurityLogManager;
      this.currentClient = currentClient;
    }

    public async Task WriteSecurityLog(string action, string authScheme)
    {
      await identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
      {
        Identity = authScheme,
        Action = action,
        ClientId = currentClient.Id,
      });
    }
  }
}
