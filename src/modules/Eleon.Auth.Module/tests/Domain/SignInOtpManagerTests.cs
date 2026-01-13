using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Common.Module.Constants;
using EleonsoftAbp.EleonsoftIdentity.Sessions;
using EleonsoftModuleCollector.Commons.Module.Messages.Identity;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Volo.Abp.Identity.Localization;
using Xunit;
using VPortal.Identity.Module.DomainServices;
using ModuleCollector.Identity.Module.Identity.Module.Domain.Sessions;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class SignInOtpManagerTests : DomainTestBase
{
    private static IStringLocalizer<IdentityResource> BuildLocalizer()
    {
        var localizer = Substitute.For<IStringLocalizer<IdentityResource>>();
        localizer["OtpMessage"].Returns(new LocalizedString("OtpMessage", "Otp message"));
        return localizer;
    }

    private static IConfiguration BuildConfig()
    {
        return new ConfigurationBuilder().Build();
    }

    private sealed class StubSessionAccessor(FullSessionInformation session) : ISessionAccessor
    {
        public FullSessionInformation Session { get; } = session;
    }

    [Fact]
    public async Task SendOtpGenerationMessage_SmsPreference_UsesPhoneOnlyAndMasks()
    {
        var bus = Substitute.For<IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
        bus.RequestAsync<IdentitySettingsResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new IdentitySettingsResponseMsg
            {
                Settings =
                [
                    new() { Name = IdentitySettingsConsts.TwoFactorAuthenticationOption, Value = IdentitySettingsConsts.TwoFactorAuthenticationOptions.Sms }
                ]
            }));

        SendOtpMsg capturedRequest = null;
        bus.RequestAsync<OtpSentMsg>(Arg.Do<object>(req => capturedRequest = (SendOtpMsg)req), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpSentMsg
            {
                Result = new() { Success = true, Message = string.Empty }
            }));

        var manager = new SignInOtpManager(
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus,
            BuildLocalizer(),
            BuildConfig(),
            new StubSessionAccessor(new FullSessionInformation { SessionId = "session" }));

        var user = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        user.SetPhoneNumber("1234567890", false);

        var result = await manager.SendOtpGenerationMessage(user);

        Assert.True(result.Success);
        Assert.NotNull(capturedRequest);
        Assert.Single(capturedRequest.Recipients);
        Assert.Equal("1234567890", capturedRequest.Recipients[0].Recipient);
        Assert.Contains("***", result.Message);
        Assert.StartsWith("123", result.Message);
        Assert.EndsWith("890", result.Message);
    }

    [Fact]
    public async Task SendOtpGenerationMessage_EmailPreference_UsesEmailWhenNoPhone()
    {
        var bus = Substitute.For<IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
        bus.RequestAsync<IdentitySettingsResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new IdentitySettingsResponseMsg
            {
                Settings =
                [
                    new() { Name = IdentitySettingsConsts.TwoFactorAuthenticationOption, Value = IdentitySettingsConsts.TwoFactorAuthenticationOptions.Email }
                ]
            }));

        SendOtpMsg capturedRequest = null;
        bus.RequestAsync<OtpSentMsg>(Arg.Do<object>(req => capturedRequest = (SendOtpMsg)req), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpSentMsg
            {
                Result = new() { Success = true, Message = string.Empty }
            }));

        var manager = new SignInOtpManager(
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus,
            BuildLocalizer(),
            BuildConfig(),
            new StubSessionAccessor(new FullSessionInformation { SessionId = "session" }));

        var user = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "user", "user@example.com");

        var result = await manager.SendOtpGenerationMessage(user);

        Assert.True(result.Success);
        Assert.NotNull(capturedRequest);
        Assert.Single(capturedRequest.Recipients);
        Assert.Equal("user@example.com", capturedRequest.Recipients[0].Recipient);
    }

    [Fact]
    public async Task SendOtpGenerationMessage_NullResponse_Throws()
    {
        var bus = Substitute.For<IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
        bus.RequestAsync<IdentitySettingsResponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new IdentitySettingsResponseMsg
            {
                Settings = []
            }));

        bus.RequestAsync<OtpSentMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpSentMsg { Result = null }));

        var manager = new SignInOtpManager(
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus,
            BuildLocalizer(),
            BuildConfig(),
            new StubSessionAccessor(new FullSessionInformation { SessionId = "session" }));

        var user = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "user", "user@example.com");

        await Assert.ThrowsAsync<Exception>(() => manager.SendOtpGenerationMessage(user));
    }

    [Fact]
    public async Task SendOtpValidationMessage_NullResult_Throws()
    {
        var bus = Substitute.For<IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
        bus.RequestAsync<OtpValidatedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpValidatedMsg { Result = null }));

        var manager = new SignInOtpManager(
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus,
            BuildLocalizer(),
            BuildConfig(),
            new StubSessionAccessor(new FullSessionInformation { SessionId = "session" }));

        await Assert.ThrowsAsync<Exception>(() => manager.SendOtpValidationMessage(Guid.NewGuid(), "000000"));
    }

    [Fact]
    public async Task SendOtpValidationMessage_ReturnsValidationResult()
    {
        var bus = Substitute.For<IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
        bus.RequestAsync<OtpValidatedMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new OtpValidatedMsg { Result = new OtpValidationResultEto(true, null) }));

        var manager = new SignInOtpManager(
            (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus,
            BuildLocalizer(),
            BuildConfig(),
            new StubSessionAccessor(new FullSessionInformation { SessionId = "session" }));

        var result = await manager.SendOtpValidationMessage(Guid.NewGuid(), "000000");

        Assert.True(result.Valid);
        Assert.Null(result.ErrorMessage);
    }
}
