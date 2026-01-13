using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.SecurityLog;
using Volo.Abp.Users;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for creating IdentitySecurityLogManager instances in tests.
/// </summary>
public static class IdentitySecurityLogTestHelpers
{
    /// <summary>
    /// Creates a real IdentitySecurityLogManager instance with all required dependencies.
    /// </summary>
    /// <param name="userManager">The identity user manager.</param>
    /// <param name="currentUser">Optional current user for the security log context.</param>
    /// <returns>A configured IdentitySecurityLogManager instance.</returns>
    public static IdentitySecurityLogManager BuildIdentitySecurityLogManager(
        IdentityUserManager userManager,
        Volo.Abp.Identity.IdentityUser currentUser = null)
    {
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Any<Action<SecurityLogInfo>>()).Returns(Task.CompletedTask);

        var principalAccessor = Substitute.For<ICurrentPrincipalAccessor>();
        principalAccessor.Change(Arg.Any<ClaimsPrincipal>()).Returns(Substitute.For<IDisposable>());

        var principalFactory = Substitute.For<IUserClaimsPrincipalFactory<Volo.Abp.Identity.IdentityUser>>();
        principalFactory.CreateAsync(Arg.Any<Volo.Abp.Identity.IdentityUser>()).Returns(Task.FromResult(new ClaimsPrincipal()));

        var currentUserAccessor = Substitute.For<ICurrentUser>();
        currentUserAccessor.IsAuthenticated.Returns(currentUser != null);
        currentUserAccessor.Id.Returns(currentUser?.Id);
        currentUserAccessor.UserName.Returns(currentUser?.UserName);

        return new IdentitySecurityLogManager(
            securityLogManager,
            userManager,
            principalAccessor,
            principalFactory,
            currentUserAccessor);
    }
}

