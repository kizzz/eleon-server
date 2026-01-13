using Logging.Module;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Migrations.Module;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace VPortal.Data;

[ExposeServices(typeof(IDataSeeder), typeof(IIdentityDataSeeder), typeof(IdentityDataSeeder))]
[Volo.Abp.DependencyInjection.Dependency(ReplaceServices = true)]
public class IdentityDataSeeder : IDataSeeder, ITransientDependency, IIdentityDataSeeder
{
  protected IGuidGenerator GuidGenerator { get; }
  protected IIdentityRoleRepository RoleRepository { get; }
  protected IIdentityUserRepository UserRepository { get; }
  protected ILookupNormalizer LookupNormalizer { get; }
  protected IdentityUserManager UserManager { get; }
  protected IdentityRoleManager RoleManager { get; }
  protected ICurrentTenant CurrentTenant { get; }
  protected IOptions<IdentityOptions> IdentityOptions { get; }
  protected IVportalLogger<IdentityDataSeeder> Logger { get; }
  protected IPermissionManager PermissionManager { get; }
  protected IPermissionDefinitionManager PermissionDefinitionManager { get; }
  protected IPermissionGrantRepository PermissionGrantRepository { get; }

  public IdentityDataSeeder(
      IGuidGenerator guidGenerator,
      IIdentityRoleRepository roleRepository,
      IIdentityUserRepository userRepository,
      ILookupNormalizer lookupNormalizer,
      IdentityUserManager userManager,
      IdentityRoleManager roleManager,
      ICurrentTenant currentTenant,
      IOptions<IdentityOptions> identityOptions,
      IVportalLogger<IdentityDataSeeder> logger,
      IPermissionManager permissionManager,
      IPermissionDefinitionManager permissionDefinitionManager,
      IPermissionGrantRepository permissionGrantRepository)
  {
    GuidGenerator = guidGenerator;
    RoleRepository = roleRepository;
    UserRepository = userRepository;
    LookupNormalizer = lookupNormalizer;
    UserManager = userManager;
    RoleManager = roleManager;
    CurrentTenant = currentTenant;
    IdentityOptions = identityOptions;
    Logger = logger;
    PermissionManager = permissionManager;
    PermissionDefinitionManager = permissionDefinitionManager;
    PermissionGrantRepository = permissionGrantRepository;
  }

  private async Task GrantAllPermissionsForAdminRoleAsync()
  {
    try
    {
      var permissions = await PermissionDefinitionManager.GetPermissionsAsync();
      var grantedDbSet = await PermissionGrantRepository.GetDbSetAsync();
      var granted = await grantedDbSet.Where(x => x.ProviderName == "R" && x.ProviderKey == MigrationConsts.AdminRoleNameDefaultValue).ToListAsync();
      var listToInsert = new List<PermissionGrant>();
      foreach (var permission in permissions)
      {
        var exists = granted.Any(g => g.Name == permission.Name && g.ProviderName == "R" && g.ProviderKey == MigrationConsts.AdminRoleNameDefaultValue);
        if (exists)
        {
          continue;
        }
        listToInsert.Add(new PermissionGrant(GuidGenerator.Create(), permission.Name, "R", MigrationConsts.AdminRoleNameDefaultValue, CurrentTenant.Id));
      }

      if (listToInsert.Count > 0)
      {
        await PermissionGrantRepository.InsertManyAsync(listToInsert, true);
      }
    }
    catch (Exception ex)
    {
      Logger.Capture(ex);
    }
  }

  private async Task AddAdminUserToAdminRole()
  {
    var adminUser = await UserRepository
        .FindByNormalizedUserNameAsync(LookupNormalizer.NormalizeName(MigrationConsts.AdminUserNameDefaultValue))
        .ConfigureAwait(false);

    var adminRole = await RoleRepository.FindByNormalizedNameAsync(LookupNormalizer.NormalizeName(MigrationConsts.AdminRoleNameDefaultValue)).ConfigureAwait(false);

    if (!adminUser.IsInRole(adminRole.Id))
    {
      (await UserManager.AddToRoleAsync(adminUser, MigrationConsts.AdminRoleNameDefaultValue).ConfigureAwait(false)).CheckErrors();
    }
  }


  private async Task<IdentityDataSeedResult> CreateAdminUserAndRoleAsync(String adminEmail, String adminPassword, Guid? tenantId)
  {
    IdentityDataSeedResult result = new IdentityDataSeedResult();
    result.CreatedAdminUser = await CreateAdminUserIfItDoesNotExistAsync(tenantId, adminEmail, adminPassword).ConfigureAwait(false);
    result.CreatedAdminRole = await CreateAdminRoleIfItDoesNotExistAsync(tenantId).ConfigureAwait(false);

    return result;
  }

  private async Task<Boolean> CreateAdminRoleIfItDoesNotExistAsync(Guid? tenantId)
  {
    var adminRole = await RoleRepository
        .FindByNormalizedNameAsync(LookupNormalizer.NormalizeName(MigrationConsts.AdminRoleNameDefaultValue))
        .ConfigureAwait(false);

    if (adminRole == null)
    {
      adminRole = new IdentityRole(GuidGenerator.Create(), MigrationConsts.AdminRoleNameDefaultValue, tenantId)
      {
        IsStatic = true,
        IsPublic = false
      };

      (await RoleManager.CreateAsync(adminRole).ConfigureAwait(false)).CheckErrors();

      Logger.Log.LogInformation("Created Role: {RoleName} ({RoleId})", adminRole.Name, adminRole.Id);

      return true;
    }

    return false;
  }

  private async Task<Boolean> CreateAdminUserIfItDoesNotExistAsync(Guid? tenantId, String adminEmail, String adminPassword)
  {
    var normalizedUserName = LookupNormalizer.NormalizeName(MigrationConsts.AdminUserNameDefaultValue);
    var normalizedEmail = LookupNormalizer.NormalizeEmail(adminEmail);

    var adminUser = await UserRepository.FindByNormalizedUserNameAsync(normalizedUserName).ConfigureAwait(false);

    if (adminUser == null)
    {
      adminUser = await UserRepository.FindByNormalizedEmailAsync(normalizedEmail).ConfigureAwait(false);
    }

    if (adminUser == null)
    {
      adminUser = new IdentityUser(GuidGenerator.Create(), MigrationConsts.AdminUserNameDefaultValue, adminEmail, tenantId)
      {
        Name = MigrationConsts.AdminUserNameDefaultValue
      };

      (await UserManager.CreateAsync(adminUser, adminPassword, validatePassword: false).ConfigureAwait(false)).CheckErrors();

      Logger.Log.LogInformation("Created User: {UserName} ({UserId})", adminUser.Name, adminUser.Id);

      return true;
    }

    return false;
  }

  public async Task<IdentityDataSeedResult> SeedAsync(string adminEmail, string adminPassword, Guid? tenantId = null, string adminUserName = null)
  {
    IdentityDataSeedResult result = new IdentityDataSeedResult();
    try
    {
      Check.NotNullOrWhiteSpace(adminEmail, nameof(adminEmail));
      Check.NotNullOrWhiteSpace(adminPassword, nameof(adminPassword));


      using (CurrentTenant.Change(tenantId))
      {
        await IdentityOptions.SetAsync().ConfigureAwait(false);

        result = await CreateAdminUserAndRoleAsync(adminEmail, adminPassword, tenantId).ConfigureAwait(false);

        await AddAdminUserToAdminRole().ConfigureAwait(false);
        await GrantAllPermissionsForAdminRoleAsync();
      }
    }
    catch (Exception ex)
    {
      Logger.Capture(ex);
    }
    finally
    {
    }

    return result;
  }

  public async Task SeedAsync(DataSeedContext context)
  {
    await SeedAsync(
        context.Properties.GetOrDefault(MigrationConsts.AdminEmailPropertyName)?.ToString() ?? MigrationConsts.AdminEmailDefaultValue,
        context.Properties.GetOrDefault(MigrationConsts.AdminPasswordPropertyName)?.ToString() ?? MigrationConsts.AdminPasswordDefaultValue,
        context.TenantId,
        context.Properties.GetOrDefault(MigrationConsts.AdminUserNamePropertyName)?.ToString() ?? MigrationConsts.AdminUserNameDefaultValue
    );
  }
}
