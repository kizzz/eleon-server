using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using VPortal.Identity.Module.TokenProviders;
using Xunit;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class ApiKeyTokenProviderTests
{
    [Fact]
    public async Task CanGenerateTwoFactorTokenAsync_AlwaysFalse()
    {
        var provider = new ApiKeyTokenProvider<IdentityUser>(
            DataProtectionProvider.Create("test"),
            Substitute.For<ILogger<DataProtectorTokenProvider<IdentityUser>>>(),
            Options.Create(new ApiKeyTokenProviderOptions()));

        var result = await provider.CanGenerateTwoFactorTokenAsync(null, new IdentityUser());

        Assert.False(result);
    }
}
