using Logging.Module;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace VPortal.Identity.Module.DomainServices
{
  public class PasswordChangeDomainService : DomainService
  {
    private readonly IVportalLogger<PasswordChangeDomainService> logger;
    private readonly IdentityUserManager identityUserManager;
    private readonly ICurrentUser currentUser;

    public PasswordChangeDomainService(
        IVportalLogger<PasswordChangeDomainService> logger,
        IdentityUserManager identityUserManager,
        ICurrentUser currentUser)
    {
      this.logger = logger;
      this.identityUserManager = identityUserManager;
      this.currentUser = currentUser;
    }

    public async Task<bool> ChangePassword(string oldPassword, string newPassword)
    {
      bool result = false;
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new Exception("Unauthorized.");
        }

        var user = await identityUserManager.FindByIdAsync(currentUser.Id.ToString());
        var passwordCorrect = await identityUserManager.CheckPasswordAsync(user, oldPassword);
        if (passwordCorrect && user != null)
        {
          (await identityUserManager.RemovePasswordAsync(user)).CheckErrors();
          (await identityUserManager.AddPasswordAsync(user, newPassword)).CheckErrors();
          result = true;
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<bool> ShouldChangePassword(Guid userId)
    {
      bool result = false;
      try
      {
        var user = await identityUserManager.GetByIdAsync(userId);
        result = user.ShouldChangePasswordOnNextLogin || await identityUserManager.ShouldPeriodicallyChangePasswordAsync(user);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
