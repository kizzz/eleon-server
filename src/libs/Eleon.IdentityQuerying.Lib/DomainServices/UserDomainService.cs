using Common.Module.Helpers;
using Logging.Module;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.TenantManagement.Module.Repositories;
using VPortal.TenantManagement.Module.ValueObjects;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class UserDomainService : DomainService
  {
    private readonly ICurrentTenant _currentTenant;
    private readonly IVportalLogger<UserDomainService> logger;
    private readonly IIdentityUserRepository identityUserRepository;
    private readonly ICommonUsersRepository commonUsersRepository;
    private readonly IPermissionManager permissionManager;
    private readonly IdentityUserManager identityUserManager;
    private readonly OrganizationUnitManager organizationUnitManager;
    private readonly IOrganizationUnitRepository organizationUnitRepository;
    private readonly ICurrentUser currentUser;

    public UserDomainService(ICurrentTenant currentTenant,
        IVportalLogger<UserDomainService> logger,
        IIdentityUserRepository identityUserRepository,
        ICommonUsersRepository commonUsersRepository,
        IPermissionManager permissionManager,
        IdentityUserManager identityUserManager,
        OrganizationUnitManager organizationUnitManager,
        IOrganizationUnitRepository organizationUnitRepository,
        ICurrentUser currentUser)
    {
      _currentTenant = currentTenant;
      this.logger = logger;
      this.identityUserRepository = identityUserRepository;
      this.commonUsersRepository = commonUsersRepository;
      this.permissionManager = permissionManager;
      this.identityUserManager = identityUserManager;
      this.organizationUnitManager = organizationUnitManager;
      this.organizationUnitRepository = organizationUnitRepository;
      this.currentUser = currentUser;
    }

    public async Task<List<IdentityUser>> GetAllUsersListAsync()
    {
      List<IdentityUser> result = new();
      try
      {
        result = await commonUsersRepository.GetListAsync("1", int.MaxValue, 0);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<IdentityUser> GetById(Guid id)
    {

      IdentityUser result = null;
      try
      {
        result = await identityUserRepository.GetAsync(id, true);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<IdentityUser> GetCurrentUser()
    {

      IdentityUser result = null;
      try
      {
        if (currentUser.Id.HasValue)
        {
          result = await identityUserManager.GetByIdAsync(currentUser.Id.Value);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<List<IdentityRole>> GetRoles(Guid id)
    {

      List<IdentityRole> result = null;
      try
      {
        result = await identityUserRepository.GetRolesAsync(id);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    public async Task<KeyValuePair<long, List<IdentityUser>>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null,
        string permissions = null,
        List<Guid> ignoredUsers = null)
    {
      KeyValuePair<long, List<IdentityUser>> result = new();
      try
      {
        result = new KeyValuePair<long, List<IdentityUser>>(
            await commonUsersRepository.GetCountAsync(filter, permissions, ignoredUsers),
            await commonUsersRepository.GetListAsync(sorting, maxResultCount, skipCount, filter, permissions, ignoredUsers));
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<DateTime?> GetUserLastLoginDate(Guid id)
    {
      DateTime? result = null;
      try
      {
        var user = await identityUserRepository.GetAsync(id, true);
        //var check = await securityLogDomainService.GetSecurityLogList(
        //    null,
        //    5,
        //    0,
        //    null,
        //    null,
        //    "LoginWithExternalProvider",
        //    user.Id);

        //if (check.Key > 0 && check.Value.Count > 0)
        //{
        //    result = check.Value.OrderByDescending(x => x.CreationTime).FirstOrDefault().CreationTime;
        //}
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;
    }

    public async Task<ImportExcelUsersValueObject> ImportExcelUsers(string csvContent)
    {
      ImportExcelUsersValueObject result = new();

      try
      {
        List<CsvUser> csvUsers = CsvHelper.GetCsvUsers(csvContent);

        foreach (var csvUser in csvUsers)
        {
          try
          {
            if (string.IsNullOrWhiteSpace(csvUser.UserName))
            {
              throw new Exception("User Name is required.");
            }

            if (!IsValidEmail(csvUser.Email))
            {
              throw new Exception("Email Name is required.");
            }

            if (!string.IsNullOrWhiteSpace(csvUser.PhoneNumber) && csvUser.PhoneNumber.Length > 16)
            {
              throw new Exception("Length should not exceed 16 characters.");
            }

            var user = new IdentityUser(
                Guid.NewGuid(),
                csvUser.UserName,
                csvUser.Email,
                CurrentTenant.Id
            )
            {
              Name = csvUser.FullName,
              //PhoneNumber = phoneNumber,
              //EmailConfirmed = true, // Assuming all emails are confirmed
              // IsActive = true, // Assuming all users are active
              //LockoutEnabled = false, // Assuming lockout is disabled for imported users
              //ShouldChangePasswordOnNextLogin = false // Assuming password change not required on next login
            };

            user.SetIsActive(true);
            user.SetEmailConfirmed(true);
            user.SetPhoneNumber(csvUser.PhoneNumber, true);

            foreach (var organizationUnitName in csvUser.OrganizationUnitNames)
            {
              // Find organization unit by name
              var organizationUnit = await organizationUnitRepository.GetAsync(organizationUnitName);
              if (organizationUnit != null)
              {
                // Add organization unit to user
                user.AddOrganizationUnit(organizationUnit.Id);
              }
              else
              {
                // Handle organization unit not found
              }
            }

            // Create user with password

            var identityResult = await identityUserManager.CreateAsync(user, csvUser.Password);
            csvUser.Status = identityResult.Succeeded ? "Ok" : "Error";
            csvUser.Message = identityResult.Errors.Select(t => t.Description).JoinAsString(", ");
          }
          catch (Exception ex)
          {
            result.Error = true;
            result.ErrorMessages.Add(ex.Message);
            csvUser.Message = ex.Message;
            csvUser.Status = "Error";
          }
        }

        result.CsvUser = CsvHelper.ConvertToCsvString(csvUsers);
      }
      catch (Exception e)
      {
        result.Error = true;
        result.ErrorMessages.Add(e.Message);
        logger.CaptureAndSuppress(e);
      }
      finally
      {
      }

      return result;
    }

    private bool IsValidEmail(string email)
    {
      try
      {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> SetNewPassword(Guid userId, string newPassword)
    {
      bool result = false;
      try
      {
        if (currentUser == null)
        {
          throw new AbpAuthorizationException();
        }
        var user = await identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
          throw new Exception(string.Format("User with id: {0} not found", userId));
        }

        var token = await identityUserManager.GeneratePasswordResetTokenAsync(user);
        var changePasswordResult = await identityUserManager.ResetPasswordAsync(user, token, newPassword);

        if (changePasswordResult.Succeeded)
        {
          result = true;
        }
        else
        {
          var errorMessages = changePasswordResult.Errors.Select(x => x.Description).ToArray();
          throw new Exception(string.Join(";", errorMessages));
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> ConfirmPhoneEmail(Guid userId, bool isConfirmPhone = false, bool isConfirmEmail = false, string otpCode = null)
    {
      bool result = false;
      try
      {
        if (currentUser == null)
        {
          throw new AbpAuthorizationException();
        }

        var user = await identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
          throw new Exception(string.Format("User with id: {0} not found", userId));
        }

        if (isConfirmPhone)
        {
          user.SetPhoneNumberConfirmed(true);
        }

        if (isConfirmEmail)
        {
          user.SetEmailConfirmed(true);
        }

        var identityResult = await identityUserManager.UpdateAsync(user);
        if (identityResult != null && !identityResult.Succeeded)
        {
          throw new Volo.Abp.UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
        }

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }
  }
}
