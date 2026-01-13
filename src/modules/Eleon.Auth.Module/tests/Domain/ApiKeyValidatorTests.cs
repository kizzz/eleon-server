using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Commons.Module.Messages.Identity;
using EleonsoftSdk.modules.Helpers.Module;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Xunit;
using Messaging.Module.ETO;
using ModuleCollector.Identity.Module.Identity.Module.Domain.ApiKey;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Domain;

public class ApiKeyValidatorTests : DomainTestBase
{
    private static IConfiguration BuildConfigWithSecretKeys(params string[] keys)
    {
        var values = new List<KeyValuePair<string, string>>();
        for (var i = 0; i < keys.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(keys[i]))
            {
                values.Add(new KeyValuePair<string, string>($"ApiKey:NotValidatedSecretKeys:{i}", keys[i]));
            }
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static IdentityApiKeyEto BuildApiKey(string apiKey, string secret)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Key = apiKey,
            KeySecret = secret,
            CreationTime = DateTime.UtcNow,
            AllowAuthorize = true
        };
    }

    private static IDistributedEventBus BuildEventBus(IdentityApiKeyEto apiKey)
    {
        var bus = Substitute.For<IResponseCapableEventBus, Volo.Abp.EventBus.Distributed.IDistributedEventBus>();
        bus.RequestAsync<ValidApiKeyReponseMsg>(Arg.Any<object>(), Arg.Any<int>())
            .Returns(Task.FromResult(new ValidApiKeyReponseMsg { ApiKey = apiKey }));
        return (Volo.Abp.EventBus.Distributed.IDistributedEventBus)bus;
    }

    [Fact]
    public async Task ValidateApiRequest_KeyNotFound_ReturnsInvalid()
    {
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(null);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var result = await validator.ValidateApiRequest("missing", "noncevalue123", DateTime.UtcNow, "sig");

        Assert.False(result.IsValid);
        Assert.Contains("Api key not found", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Null(result.ApiKey);
    }

    [Fact]
    public async Task ValidateApiRequest_NotValidatedSecretKey_BypassesValidation()
    {
        var apiKey = "bypass-key";
        var key = BuildApiKey(apiKey, "secret");
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys(apiKey);
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var result = await validator.ValidateApiRequest(apiKey, "short", DateTime.UtcNow.AddHours(-5), "bad");

        Assert.True(result.IsValid);
        Assert.NotNull(result.ApiKey);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task ValidateApiRequest_InvalidTimestamp_ReturnsInvalid()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var oldTimestamp = DateTime.UtcNow.AddMinutes(-10);
        var nonce = "validnonce123";
        var signature = ApiKeySignatureHelper.GenerateSignature(secret, nonce, oldTimestamp);

        var result = await validator.ValidateApiRequest(apiKey, nonce, oldTimestamp, signature);

        Assert.False(result.IsValid);
        Assert.Contains("Timestamp", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateApiRequest_TimestampWithinAllowedMistake_Accepts()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var timestamp = DateTime.UtcNow.AddSeconds(9);
        var nonce = "validnonce123";
        var signature = ApiKeySignatureHelper.GenerateSignature(secret, nonce, timestamp);

        var result = await validator.ValidateApiRequest(apiKey, nonce, timestamp, signature.ToLowerInvariant());

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateApiRequest_TimestampTooFarInFuture_Rejects()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var timestamp = DateTime.UtcNow.AddSeconds(20);
        var nonce = "validnonce123";
        var signature = ApiKeySignatureHelper.GenerateSignature(secret, nonce, timestamp);

        var result = await validator.ValidateApiRequest(apiKey, nonce, timestamp, signature);

        Assert.False(result.IsValid);
        Assert.Contains("Timestamp", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateApiRequest_NonceTooShort_Rejects()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var timestamp = DateTime.UtcNow;
        var nonce = "short";
        var signature = ApiKeySignatureHelper.GenerateSignature(secret, nonce, timestamp);

        var result = await validator.ValidateApiRequest(apiKey, nonce, timestamp, signature);

        Assert.False(result.IsValid);
        Assert.Contains("Nonce", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateApiRequest_InvalidSignature_ReturnsInvalid()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var timestamp = DateTime.UtcNow;
        var nonce = "validnonce123";

        var result = await validator.ValidateApiRequest(apiKey, nonce, timestamp, "invalid-signature");

        Assert.False(result.IsValid);
        Assert.Contains("signature", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateApiRequest_NonceReused_IsRejected()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var timestamp = DateTime.UtcNow;
        var nonce = "validnonce123";
        var signature = ApiKeySignatureHelper.GenerateSignature(secret, nonce, timestamp);

        var first = await validator.ValidateApiRequest(apiKey, nonce, timestamp, signature);
        var second = await validator.ValidateApiRequest(apiKey, nonce, timestamp, signature);

        Assert.True(first.IsValid);
        Assert.False(second.IsValid);
        Assert.Contains("Nonce", second.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateApiRequest_ValidRequest_ReturnsValid()
    {
        var apiKey = "key";
        var secret = "secret";
        var key = BuildApiKey(apiKey, secret);
        var memoryCache = CreateMemoryCache();
        var eventBus = BuildEventBus(key);
        var config = BuildConfigWithSecretKeys();
        var validator = new ApiKeyValidator(CreateMockLogger<ApiKeyValidator>(), eventBus, memoryCache, config);

        var timestamp = DateTime.UtcNow;
        var nonce = "validnonce123";
        var signature = ApiKeySignatureHelper.GenerateSignature(secret, nonce, timestamp);

        var result = await validator.ValidateApiRequest(apiKey, nonce, timestamp, signature);

        Assert.True(result.IsValid);
        Assert.NotNull(result.ApiKey);
        Assert.Equal(apiKey, result.ApiKey.Key);
    }
}
