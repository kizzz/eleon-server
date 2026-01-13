using Logging.Module;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace VPortal.Identity.Module.DomainServices;

[ExposeServices(typeof(EventUserManager), typeof(IdentityUserManager), typeof(UserManager<IdentityUser>))]
public class EventUserManager : IdentityUserManager, IScopedDependency // relaize the override of this methods with events to divide database for auth and admin
{
  private readonly IDistributedEventBus _distributedEventBus;
  private readonly IVportalLogger<EventUserManager> _logger;

  public EventUserManager(
      IdentityUserStore store,
      IIdentityRoleRepository roleRepository,
      IIdentityUserRepository userRepository,
      Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor,
      IPasswordHasher<IdentityUser> passwordHasher,
      IEnumerable<IUserValidator<IdentityUser>> userValidators,
      IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
      ILookupNormalizer keyNormalizer,
      IdentityErrorDescriber errors,
      IServiceProvider services,
      Microsoft.Extensions.Logging.ILogger<IdentityUserManager> logger,
      ICancellationTokenProvider cancellationTokenProvider,
      IOrganizationUnitRepository organizationUnitRepository,
      ISettingProvider settingProvider,
      IDistributedEventBus distributedEventBus,
      IIdentityLinkUserRepository identityLinkUserRepository,
      IDistributedCache<AbpDynamicClaimCacheItem> dynamicClaimCache,
      IVportalLogger<EventUserManager> vportalLogger
  )
      : base(
          store,
          roleRepository,
          userRepository,
          optionsAccessor,
          passwordHasher,
          userValidators,
          passwordValidators,
          keyNormalizer,
          errors,
          services,
          logger,
          cancellationTokenProvider,
          organizationUnitRepository,
          settingProvider,
          distributedEventBus,
          identityLinkUserRepository,
          dynamicClaimCache
      )
  {
    _distributedEventBus = distributedEventBus;
    _logger = vportalLogger;
  }

  public override async Task<IdentityUser> FindByIdAsync(string userId) // Identity Server in ProfileService<T> // required by identity server
  {
    // Call main service or cache to get user
    return await base.FindByIdAsync(userId);
  }

  public override async Task<IdentityUser> FindByNameAsync(string userName)  // required by identity server
  {
    // Call main service
    return await base.FindByNameAsync(userName);
  }

  public override Task<IdentityUser> FindByEmailAsync(string email)
  {
    return base.FindByEmailAsync(email);
  }

  public override async Task<bool> CheckPasswordAsync(IdentityUser user, string password)  // required by identity server
  {
    // Delegate password check to external service
    return await base.CheckPasswordAsync(user, password);
  }

  public override async Task<IList<Claim>> GetClaimsAsync(IdentityUser user)
  {
    return await base.GetClaimsAsync(user);
  }

  public override async Task<bool> IsLockedOutAsync(IdentityUser user)
  {
    return await base.IsLockedOutAsync(user);
  }

  public override async Task<bool> GetTwoFactorEnabledAsync(IdentityUser user)
  {
    return await base.GetTwoFactorEnabledAsync(user);
  }

  public override Task<IdentityResult> RemovePasswordAsync(IdentityUser user)
  {
    return base.RemovePasswordAsync(user);
  }

  public override Task<IdentityResult> AddPasswordAsync(IdentityUser user, string password)
  {
    return base.AddPasswordAsync(user, password);
  }

  public override Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName)
  {
    return base.GetUsersInRoleAsync(roleName);
  }

  public override Task<IdentityResult> CreateAsync(IdentityUser user)
  {
    return base.CreateAsync(user);
  }

  public override Task<IdentityResult> CreateAsync(IdentityUser user, string password)
  {
    return base.CreateAsync(user, password);
  }

  public override Task<IdentityResult> CreateAsync(IdentityUser user, string password, bool validatePassword)
  {
    return base.CreateAsync(user, password, validatePassword);
  }

  public override Task<IdentityResult> UpdateSecurityStampAsync(IdentityUser user)
  {
    return base.UpdateSecurityStampAsync(user);
  }

  public override Task<IdentityResult> UpdateAsync(IdentityUser user)
  {
    return base.UpdateAsync(user);
  }

  public override Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword)
  {
    return base.ResetPasswordAsync(user, token, newPassword);
  }

  public override Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
  {
    return base.GeneratePasswordResetTokenAsync(user);
  }

  public override Task<bool> ShouldPeriodicallyChangePasswordAsync(IdentityUser user)
  {
    return base.ShouldPeriodicallyChangePasswordAsync(user);
  }

  public override Task<bool> IsInRoleAsync(IdentityUser user, string role)
  {
    return base.IsInRoleAsync(user, role);
  }

  public override Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey)
  {
    return base.FindByLoginAsync(loginProvider, providerKey);
  }

  public override Task<IdentityResult> AccessFailedAsync(IdentityUser user)
  {
    return base.AccessFailedAsync(user);
  }
  // https://github.com/DuendeSoftware/products/blob/b7da7115544576883e953d28d441b48565d95ce2/identity-server/src/AspNetIdentity/IdentityServerBuilderExtensions.cs#L31
  // https://github.com/DuendeSoftware/products/blob/main/identity-server/src/AspNetIdentity/ProfileService.cs#L19
  // https://github.com/DuendeSoftware/products/blob/main/identity-server/src/AspNetIdentity/ResourceOwnerPasswordValidator.cs#L18
  // https://github.com/DuendeSoftware/products/blob/main/identity-server/src/AspNetIdentity/UserClaimsFactory.cs#L12
  public override Task<string> GetUserIdAsync(IdentityUser user) // identity server in UserClaimsFactory<T> for ProfileService<T>  // required by identity server
  {
    return base.GetUserIdAsync(user);
  }

  public override Task<string> GetUserNameAsync(IdentityUser user) // identity server in UserClaimsFactory<T> for ProfileService<T>
  {
    return base.GetUserNameAsync(user);
  }

  public override bool SupportsUserEmail => base.SupportsUserEmail; // identity server in UserClaimsFactory<T> for ProfileService<T>

  public override Task<bool> IsEmailConfirmedAsync(IdentityUser user) // identity server in UserClaimsFactory<T> for ProfileService<T>
  {
    return base.IsEmailConfirmedAsync(user);
  }

  public override bool SupportsUserPhoneNumber => base.SupportsUserPhoneNumber; // identity server in UserClaimsFactory<T> for ProfileService<T>

  public override Task<string> GetPhoneNumberAsync(IdentityUser user) // identity server in UserClaimsFactory<T> for ProfileService<T>
  {
    return base.GetPhoneNumberAsync(user);
  }

  public override Task<bool> IsPhoneNumberConfirmedAsync(IdentityUser user) // identity server in UserClaimsFactory<T> for ProfileService<T>
  {
    return base.IsPhoneNumberConfirmedAsync(user);
  }
}
