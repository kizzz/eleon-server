using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;
using VPortal.Identity.Module.SignIn;
using IdentityRole = Volo.Abp.Identity.IdentityRole;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using Eleon.TestsBase.Lib.TestHelpers;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;

internal sealed class TestIdentityUserStore : IdentityUserStore
{
    private static readonly IIdentityUserRepository MockUserRepo = Substitute.For<IIdentityUserRepository>();
    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly IGuidGenerator MockGuidGenerator = Substitute.For<IGuidGenerator>();
    private static readonly ILogger<IdentityRoleStore> MockLogger = Substitute.For<ILogger<IdentityRoleStore>>();
    private static readonly ILookupNormalizer MockNormalizer = Substitute.For<ILookupNormalizer>();
    private static readonly IdentityErrorDescriber LocalErrorDescriber = new IdentityErrorDescriber();
    private readonly Func<IdentityUser, IReadOnlyCollection<string>> _getRoles;

    public TestIdentityUserStore()
        : base(MockUserRepo, MockRoleRepo, MockGuidGenerator, MockLogger, MockNormalizer, LocalErrorDescriber)
    {
    }

    public TestIdentityUserStore(
        IIdentityUserRepository userRepository,
        IIdentityRoleRepository roleRepository,
        Func<IdentityUser, IReadOnlyCollection<string>> getRoles = null)
        : base(userRepository, roleRepository, MockGuidGenerator, MockLogger, MockNormalizer, LocalErrorDescriber)
    {
        _getRoles = getRoles;
    }

    public override Task<IList<string>> GetRolesAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken = default)
    {
        if (_getRoles == null)
        {
            return base.GetRolesAsync(user, cancellationToken);
        }

        var roles = _getRoles(user) ?? Array.Empty<string>();
        return Task.FromResult<IList<string>>(roles.ToList());
    }

    public override Task<bool> IsInRoleAsync(IdentityUser user, string normalizedRoleName, System.Threading.CancellationToken cancellationToken = default)
    {
        if (_getRoles == null)
        {
            return base.IsInRoleAsync(user, normalizedRoleName, cancellationToken);
        }

        if (user == null || string.IsNullOrWhiteSpace(normalizedRoleName))
        {
            return Task.FromResult(false);
        }

        var roles = _getRoles(user) ?? Array.Empty<string>();
        return Task.FromResult(roles.Any(role => string.Equals(role, normalizedRoleName, StringComparison.OrdinalIgnoreCase)));
    }
}

internal sealed class TestIdentityRoleStore : IdentityRoleStore
{
    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly ILogger<IdentityRoleStore> MockLogger = Substitute.For<ILogger<IdentityRoleStore>>();
    private static readonly IGuidGenerator MockGuidGenerator = Substitute.For<IGuidGenerator>();
    private static readonly IdentityErrorDescriber LocalErrorDescriber = new IdentityErrorDescriber();

    public TestIdentityRoleStore()
        : base(MockRoleRepo, MockLogger, MockGuidGenerator, LocalErrorDescriber)
    {
    }
}

public class TestIdentityUserManager : IdentityUserManager
{
    private readonly Dictionary<Guid, IdentityUser> _users = new();
    private readonly Dictionary<string, IdentityUser> _usersByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, IdentityUser> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Guid, HashSet<string>> _userRoles = new();
    private IdentityUser _getByIdResult;

    private static readonly IIdentityRoleRepository MockRoleRepo = Substitute.For<IIdentityRoleRepository>();
    private static readonly IIdentityUserRepository MockUserRepo = Substitute.For<IIdentityUserRepository>();
    private static readonly IPasswordHasher<IdentityUser> MockPasswordHasher = Substitute.For<IPasswordHasher<IdentityUser>>();
    private static readonly ILookupNormalizer MockNormalizer = Substitute.For<ILookupNormalizer>();
    private static readonly IdentityErrorDescriber LocalErrorDescriber = new IdentityErrorDescriber();
    private static readonly IServiceProvider MockServiceProvider = Substitute.For<IServiceProvider>();
    private static readonly ILogger<IdentityUserManager> MockLogger = Substitute.For<ILogger<IdentityUserManager>>();
    private static readonly ICancellationTokenProvider MockCancellationTokenProvider = Substitute.For<ICancellationTokenProvider>();
    private static readonly IOrganizationUnitRepository MockOrgUnitRepo = Substitute.For<IOrganizationUnitRepository>();
    private static readonly ISettingProvider MockSettingProvider = Substitute.For<ISettingProvider>();
    private static readonly IDistributedEventBus MockEventBus = Substitute.For<IDistributedEventBus>();
    private static readonly IIdentityLinkUserRepository MockLinkUserRepo = Substitute.For<IIdentityLinkUserRepository>();
    private static readonly IDistributedCache<AbpDynamicClaimCacheItem> MockCache = Substitute.For<IDistributedCache<AbpDynamicClaimCacheItem>>();

    public TestIdentityUserManager()
        : base(
            new TestIdentityUserStore(),
            MockRoleRepo,
            MockUserRepo,
            OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>(),
            MockPasswordHasher,
            new List<IUserValidator<IdentityUser>>(),
            new List<IPasswordValidator<IdentityUser>>(),
            MockNormalizer,
            LocalErrorDescriber,
            MockServiceProvider,
            MockLogger,
            MockCancellationTokenProvider,
            MockOrgUnitRepo,
            MockSettingProvider,
            MockEventBus,
            MockLinkUserRepo,
            MockCache)
    {
    }

    public void AddUser(IdentityUser user)
    {
        _users[user.Id] = user;
        if (!string.IsNullOrWhiteSpace(user.UserName))
        {
            _usersByName[user.UserName] = user;
        }
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            _usersByEmail[user.Email] = user;
        }
    }

    public IReadOnlyCollection<string> GetRolesForUser(IdentityUser user)
    {
        if (user == null)
        {
            return Array.Empty<string>();
        }

        return _userRoles.TryGetValue(user.Id, out var roles)
            ? roles.ToList()
            : Array.Empty<string>();
    }

    public override Task<IdentityUser> FindByIdAsync(string userId)
    {
        if (_getByIdResult != null)
        {
            return Task.FromResult(_getByIdResult);
        }

        if (Guid.TryParse(userId, out var id) && _users.TryGetValue(id, out var user))
        {
            return Task.FromResult(user);
        }

        return Task.FromResult<IdentityUser>(null);
    }

    public override Task<IdentityUser> FindByNameAsync(string userName)
    {
        if (!string.IsNullOrWhiteSpace(userName) && _usersByName.TryGetValue(userName, out var user))
        {
            return Task.FromResult(user);
        }

        return Task.FromResult<IdentityUser>(null);
    }

    public override Task<IdentityUser> FindByEmailAsync(string email)
    {
        if (!string.IsNullOrWhiteSpace(email) && _usersByEmail.TryGetValue(email, out var user))
        {
            return Task.FromResult(user);
        }

        return Task.FromResult<IdentityUser>(null);
    }

    public override Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
    {
        if (user == null || string.IsNullOrWhiteSpace(roleName))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(_userRoles.TryGetValue(user.Id, out var roles) && roles.Contains(roleName));
    }

    public override Task<IdentityResult> AddToRoleAsync(IdentityUser user, string roleName)
    {
        if (user == null || string.IsNullOrWhiteSpace(roleName))
        {
            return Task.FromResult(IdentityResult.Failed());
        }

        if (!_userRoles.TryGetValue(user.Id, out var roles))
        {
            roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _userRoles[user.Id] = roles;
        }

        roles.Add(roleName);
        return Task.FromResult(IdentityResult.Success);
    }

    private bool _passwordCheckResult = true;
    private bool _shouldPeriodicallyChangePassword = false;
    private Exception _exceptionOnRemovePassword;
    public bool RemovePasswordCalled { get; private set; }
    public bool AddPasswordCalled { get; private set; }
    public string LastPasswordAdded { get; private set; }
    private IdentityResult _resetPasswordResult = IdentityResult.Success;

    public void SetResetPasswordResult(IdentityResult result)
    {
        _resetPasswordResult = result ?? IdentityResult.Failed();
    }

    public void SetPasswordCheckResult(bool result)
    {
        _passwordCheckResult = result;
    }

    public bool GetPasswordCheckResult()
    {
        return _passwordCheckResult;
    }

    public void SetShouldPeriodicallyChangePassword(bool shouldChange)
    {
        _shouldPeriodicallyChangePassword = shouldChange;
    }

    public void SetExceptionOnRemovePassword(Exception ex)
    {
        _exceptionOnRemovePassword = ex;
    }

    public override Task<bool> CheckPasswordAsync(IdentityUser user, string password)
    {
        return Task.FromResult(_passwordCheckResult);
    }

    public override Task<IdentityResult> RemovePasswordAsync(IdentityUser user)
    {
        if (_exceptionOnRemovePassword != null)
        {
            throw _exceptionOnRemovePassword;
        }
        RemovePasswordCalled = true;
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> AddPasswordAsync(IdentityUser user, string password)
    {
        AddPasswordCalled = true;
        LastPasswordAdded = password;
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
    {
        return Task.FromResult("reset-token");
    }

    public override Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword)
    {
        return Task.FromResult(_resetPasswordResult);
    }

    public override Task<IdentityUser> GetByIdAsync(Guid id)
    {
        if (_getByIdResult != null)
        {
            return Task.FromResult(_getByIdResult);
        }

        if (_users.TryGetValue(id, out var user))
        {
            return Task.FromResult(user);
        }
        return Task.FromResult<IdentityUser>(null);
    }

    public void SetGetByIdResult(IdentityUser user)
    {
        _getByIdResult = user;
    }

    public override Task<bool> ShouldPeriodicallyChangePasswordAsync(IdentityUser user)
    {
        return Task.FromResult(_shouldPeriodicallyChangePassword);
    }

    public override Task<IdentityResult> UpdateAsync(IdentityUser user)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> UpdateSecurityStampAsync(IdentityUser user)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    private IdentityUser _findByLoginResult;
    public void SetFindByLoginResult(IdentityUser user)
    {
        _findByLoginResult = user;
    }

    public override Task<IdentityUser> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return Task.FromResult(_findByLoginResult);
    }

    public override Task<IdentityResult> AddLoginAsync(IdentityUser user, UserLoginInfo login)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> CreateAsync(IdentityUser user)
    {
        var result = GetCreateResult(user);
        if (result.Succeeded)
        {
            AddUser(user);
        }
        return Task.FromResult(result);
    }

    public override Task<IdentityResult> CreateAsync(IdentityUser user, string password)
    {
        return CreateAsync(user);
    }

    public override Task<IdentityResult> CreateAsync(IdentityUser user, string password, bool validatePassword)
    {
        return CreateAsync(user);
    }

    public override Task<IdentityResult> SetEmailAsync(IdentityUser user, string email)
    {
        if (user != null)
        {
            typeof(IdentityUser).GetProperty("Email")?.SetValue(user, email);
            if (!string.IsNullOrWhiteSpace(email))
            {
                _usersByEmail[email] = user;
            }
        }
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> AddDefaultRolesAsync(IdentityUser user)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public override Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
    {
        if (_passwordCheckResult)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        throw new InvalidOperationException("Invalid password");
    }

    private IdentityResult GetCreateResult(IdentityUser user)
    {
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User is null" });
        }

        if (!string.IsNullOrWhiteSpace(user.UserName) && _usersByName.ContainsKey(user.UserName))
        {
            return IdentityResult.Failed(new IdentityError { Description = "User name already exists" });
        }

        if (!string.IsNullOrWhiteSpace(user.Email) && _usersByEmail.ContainsKey(user.Email))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Email already exists" });
        }

        return IdentityResult.Success;
    }
}

public sealed class TestSignInManager : SignInManager
{
    private readonly ClaimsPrincipal _principal;
    public ExternalLoginInfo ExternalLoginInfo { get; set; }
    public IReadOnlyList<Claim> CapturedClaims { get; private set; } = Array.Empty<Claim>();

    public TestSignInManager(IdentityUserManager userManager, ClaimsPrincipal principal)
        : base(
            userManager,
            new HttpContextAccessor(),
            Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>(),
            OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<IdentityOptions>(),
            Substitute.For<ILogger<SignInManager<IdentityUser>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<IdentityUser>>(),
            OptionsTestHelpers.CreateTestAbpDynamicOptionsManager<AbpIdentityOptions>(),
            Substitute.For<ISettingProvider>())
    {
        _principal = principal;
    }

    public override Task<ClaimsPrincipal> CreateUserPrincipalAsync(IdentityUser user)
    {
        return Task.FromResult(_principal);
    }

    public override Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null)
    {
        return Task.FromResult(ExternalLoginInfo);
    }

    public override Task SignInWithClaimsAsync(IdentityUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        CapturedClaims = additionalClaims?.ToList() ?? new List<Claim>();
        return Task.CompletedTask;
    }
}
