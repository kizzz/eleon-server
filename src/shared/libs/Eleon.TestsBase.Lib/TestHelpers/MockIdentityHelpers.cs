using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Minimal stub implementation of IdentityUserStore that doesn't require DI.
/// All dependencies are mocked with NSubstitute to avoid any DI container dependencies.
/// </summary>
internal sealed class MockIdentityUserStore : IdentityUserStore
{
    private static readonly IIdentityUserRepository MockUserRepo = Substitute.For<IIdentityUserRepository>();
    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly IGuidGenerator MockGuidGenerator = Substitute.For<IGuidGenerator>();
    private static readonly ILogger<IdentityRoleStore> MockLogger = Substitute.For<ILogger<IdentityRoleStore>>();
    private static readonly ILookupNormalizer MockNormalizer = Substitute.For<ILookupNormalizer>();
    private static readonly IdentityErrorDescriber MockErrorDescriber = new IdentityErrorDescriber();

    public MockIdentityUserStore()
        : base(
            MockUserRepo,
            MockRoleRepo,
            MockGuidGenerator,
            MockLogger,
            MockNormalizer,
            MockErrorDescriber)
    {
    }
}

/// <summary>
/// Minimal stub implementation of IdentityRoleStore that doesn't require DI.
/// All dependencies are mocked with NSubstitute to avoid any DI container dependencies.
/// </summary>
internal sealed class MockIdentityRoleStore : IdentityRoleStore
{
    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly ILogger<IdentityRoleStore> MockLogger = Substitute.For<ILogger<IdentityRoleStore>>();
    private static readonly IGuidGenerator MockGuidGenerator = Substitute.For<IGuidGenerator>();
    private static readonly IdentityErrorDescriber MockErrorDescriber = new IdentityErrorDescriber();

    public MockIdentityRoleStore()
        : base(
            MockRoleRepo,
            MockLogger,
            MockGuidGenerator,
            MockErrorDescriber)
    {
    }
}

/// <summary>
/// Helper class for creating mocked IdentityUserManager and IdentityRoleManager
/// that work without a real database using in-memory dictionaries.
/// </summary>
public class MockIdentityUserManager : IdentityUserManager
{
    private readonly Dictionary<Guid, IdentityUser> _users;
    private readonly Dictionary<Guid, List<string>> _userRoles;

    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly IIdentityUserRepository MockUserRepo = Substitute.For<IIdentityUserRepository>();
    private static readonly IPasswordHasher<IdentityUser> MockPasswordHasher = Substitute.For<IPasswordHasher<IdentityUser>>();
    private static readonly ILookupNormalizer MockNormalizer = Substitute.For<ILookupNormalizer>();
    private static readonly IdentityErrorDescriber MockErrorDescriber = new IdentityErrorDescriber();
    private static readonly IServiceProvider MockServiceProvider = Substitute.For<IServiceProvider>();
    private static readonly ILogger<IdentityUserManager> MockLogger = Substitute.For<ILogger<IdentityUserManager>>();
    private static readonly ICancellationTokenProvider MockCancellationTokenProvider = Substitute.For<ICancellationTokenProvider>();
    private static readonly IOrganizationUnitRepository MockOrgUnitRepo = Substitute.For<IOrganizationUnitRepository>();
    private static readonly ISettingProvider MockSettingProvider = Substitute.For<ISettingProvider>();
    private static readonly IDistributedEventBus MockEventBus = Substitute.For<IDistributedEventBus>();
    private static readonly IIdentityLinkUserRepository MockLinkUserRepo = Substitute.For<IIdentityLinkUserRepository>();
    private static readonly IDistributedCache<AbpDynamicClaimCacheItem> MockCache = Substitute.For<IDistributedCache<AbpDynamicClaimCacheItem>>();

    public MockIdentityUserManager(
        Dictionary<Guid, IdentityUser> users = null,
        Dictionary<Guid, List<string>> userRoles = null)
        : base(
            new MockIdentityUserStore(),
            MockRoleRepo,
            MockUserRepo,
            Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
            MockPasswordHasher,
            new List<IUserValidator<IdentityUser>>(),
            new List<IPasswordValidator<IdentityUser>>(),
            MockNormalizer,
            MockErrorDescriber,
            MockServiceProvider,
            MockLogger,
            MockCancellationTokenProvider,
            MockOrgUnitRepo,
            MockSettingProvider,
            MockEventBus,
            MockLinkUserRepo,
            MockCache)
    {
        _users = users ?? new Dictionary<Guid, IdentityUser>();
        _userRoles = userRoles ?? new Dictionary<Guid, List<string>>();
    }

    public override Task<IdentityUser> FindByIdAsync(string userId)
    {
        if (Guid.TryParse(userId, out var id) && _users.TryGetValue(id, out var user))
        {
            return Task.FromResult(user);
        }

        return Task.FromResult<IdentityUser>(null);
    }

    public override Task<IdentityUser> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_users.TryGetValue(id, out var user) ? user : null);
    }

    public override Task<IList<string>> GetRolesAsync(IdentityUser user)
    {
        if (user != null && _userRoles.TryGetValue(user.Id, out var roles))
        {
            return Task.FromResult<IList<string>>(roles);
        }

        return Task.FromResult<IList<string>>(new List<string>());
    }

    public override Task<bool> IsInRoleAsync(IdentityUser user, string role)
    {
        if (user == null || string.IsNullOrWhiteSpace(role))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_userRoles.TryGetValue(user.Id, out var roles)
            && roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase)));
    }

    public void AddUser(IdentityUser user)
    {
        _users[user.Id] = user;
    }

    public void AddUserRole(Guid userId, string roleName)
    {
        if (!_userRoles.TryGetValue(userId, out var roles))
        {
            roles = new List<string>();
            _userRoles[userId] = roles;
        }

        if (!roles.Contains(roleName))
        {
            roles.Add(roleName);
        }
    }
}

public class MockIdentityRoleManager : IdentityRoleManager
{
    private readonly Dictionary<string, IdentityRole> _rolesByName;
    private readonly Dictionary<Guid, IdentityRole> _rolesById;

    private static readonly ILookupNormalizer MockNormalizer = Substitute.For<ILookupNormalizer>();
    private static readonly IdentityErrorDescriber MockErrorDescriber = new IdentityErrorDescriber();
    private static readonly ILogger<IdentityRoleManager> MockLogger = Substitute.For<ILogger<IdentityRoleManager>>();
    private static readonly Microsoft.Extensions.Localization.IStringLocalizer<IdentityResource> MockLocalizer =
        Substitute.For<Microsoft.Extensions.Localization.IStringLocalizer<IdentityResource>>();
    private static readonly ICancellationTokenProvider MockCancellationTokenProvider = Substitute.For<ICancellationTokenProvider>();
    private static readonly IIdentityUserRepository MockUserRepo = Substitute.For<IIdentityUserRepository>();
    private static readonly IOrganizationUnitRepository MockOrgUnitRepo = Substitute.For<IOrganizationUnitRepository>();
    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly IDistributedCache<AbpDynamicClaimCacheItem> MockCache = Substitute.For<IDistributedCache<AbpDynamicClaimCacheItem>>();
    private static readonly OrganizationUnitManager MockOrgUnitManager = new OrganizationUnitManager(
        MockOrgUnitRepo,
        MockLocalizer,
        MockRoleRepo,
        MockCache,
        MockCancellationTokenProvider);

    public MockIdentityRoleManager(
        Dictionary<string, IdentityRole> rolesByName = null,
        Dictionary<Guid, IdentityRole> rolesById = null)
        : base(
            new MockIdentityRoleStore(),
            new List<IRoleValidator<IdentityRole>>(),
            MockNormalizer,
            MockErrorDescriber,
            MockLogger,
            MockLocalizer,
            MockCancellationTokenProvider,
            MockUserRepo,
            MockOrgUnitRepo,
            MockOrgUnitManager,
            MockCache)
    {
        _rolesByName = rolesByName ?? new Dictionary<string, IdentityRole>();
        _rolesById = rolesById ?? new Dictionary<Guid, IdentityRole>();
    }

    public override Task<IdentityRole> FindByNameAsync(string normalizedRoleName)
    {
        if (_rolesByName.TryGetValue(normalizedRoleName, out var role))
        {
            return Task.FromResult(role);
        }

        var match = _rolesByName.Values.FirstOrDefault(r =>
            string.Equals(r.Name, normalizedRoleName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(match);
    }

    public override Task<IdentityRole> FindByIdAsync(string roleId)
    {
        if (Guid.TryParse(roleId, out var id) && _rolesById.TryGetValue(id, out var role))
        {
            return Task.FromResult(role);
        }

        return Task.FromResult<IdentityRole>(null);
    }

    public void AddRole(IdentityRole role)
    {
        _rolesByName[role.Name] = role;
        _rolesById[role.Id] = role;
    }
}
