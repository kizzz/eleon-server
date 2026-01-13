using Common.Module.Constants;
using Common.Module.Extensions;
using Logging.Module;
using Migrations.Module;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.TenantManagement.Module.Entities;
using VPortal.TenantManagement.Module.Repositories;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class UserOtpSettingsDomainService : DomainService
  {
    private readonly IVportalLogger<UserOtpSettingsDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly IdentityUserManager identityUserManager;
    private readonly IUserOtpSettingsRepository userOtpSettingsRepository;

    public UserOtpSettingsDomainService(
        IVportalLogger<UserOtpSettingsDomainService> logger,
        ICurrentUser currentUser,
        IdentityUserManager identityUserManager,
        IUserOtpSettingsRepository userOtpSettingsRepository)
    {
      this.logger = logger;
      this.currentUser = currentUser;
      this.identityUserManager = identityUserManager;
      this.userOtpSettingsRepository = userOtpSettingsRepository;
    }

    public async Task<UserOtpSettingsEntity> GetUserOtpSettings(Guid userId)
    {
      UserOtpSettingsEntity result = null;
      try
      {
        var setting = await userOtpSettingsRepository.GetByUserIdAsync(userId);
        if (setting == null)
        {
          var identityUser = await identityUserManager.GetByIdAsync(userId);
          var otpType = UserOtpType.None;
          if (identityUser.Email.NonEmpty() && identityUser.PhoneNumber.NonEmpty())
          {
            otpType = UserOtpType.Mixed;
          }
          else if (identityUser.Email.NonEmpty())
          {
            otpType = UserOtpType.Email;
          }
          else if (identityUser.PhoneNumber.NonEmpty())
          {
            otpType = UserOtpType.Sms;
          }

          setting = new UserOtpSettingsEntity(GuidGenerator.Create())
          {
            UserId = userId,
            UserOtpType = otpType,
            OtpEmail = identityUser.Email,
            OtpPhoneNumber = identityUser.PhoneNumber,
          };

          await userOtpSettingsRepository.InsertAsync(setting, true);
        }

        result = setting;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task SetUserOtpSettings(UserOtpSettingsEntity settings)
    {
      try
      {
        var actorUser = await identityUserManager.GetByIdAsync(currentUser.Id.Value);
        bool isActorAdmin = await identityUserManager.IsInRoleAsync(actorUser, MigrationConsts.AdminRoleNameDefaultValue);
        bool isEditingSelfSettings = currentUser.Id.Value != settings.UserId;
        if (!isActorAdmin && !isEditingSelfSettings)
        {
          throw new Exception("You are not allowed to change these settings");
        }

        var existing = await GetUserOtpSettings(settings.UserId);
        existing.UserOtpType = settings.UserOtpType;
        existing.OtpEmail = settings.OtpEmail;
        existing.OtpPhoneNumber = settings.OtpPhoneNumber;
        await userOtpSettingsRepository.UpdateAsync(existing);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }
  }
}
