using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Common.EventBus.Module;
using Commons.Module.Messages.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.Devices;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.IdentityServer.Grants;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Security.Claims;
using Volo.Abp.SecurityLog;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.EventServices.Sessions;
using VPortal.Identity.Module.Sessions;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.Application;

public class SessionEventServiceTests : DomainTestBase
{
    private sealed class TestIdentityServerDbContext : AbpDbContext<TestIdentityServerDbContext>, IIdentityServerDbContext
    {
        public TestIdentityServerDbContext(DbContextOptions<TestIdentityServerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApiResource> ApiResources => Set<ApiResource>();
        public DbSet<ApiResourceSecret> ApiResourceSecrets => Set<ApiResourceSecret>();
        public DbSet<ApiResourceClaim> ApiResourceClaims => Set<ApiResourceClaim>();
        public DbSet<ApiResourceScope> ApiResourceScopes => Set<ApiResourceScope>();
        public DbSet<ApiResourceProperty> ApiResourceProperties => Set<ApiResourceProperty>();
        public DbSet<ApiScope> ApiScopes => Set<ApiScope>();
        public DbSet<ApiScopeClaim> ApiScopeClaims => Set<ApiScopeClaim>();
        public DbSet<ApiScopeProperty> ApiScopeProperties => Set<ApiScopeProperty>();
        public DbSet<IdentityResource> IdentityResources => Set<IdentityResource>();
        public DbSet<IdentityResourceClaim> IdentityClaims => Set<IdentityResourceClaim>();
        public DbSet<IdentityResourceProperty> IdentityResourceProperties => Set<IdentityResourceProperty>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<ClientGrantType> ClientGrantTypes => Set<ClientGrantType>();
        public DbSet<ClientRedirectUri> ClientRedirectUris => Set<ClientRedirectUri>();
        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris => Set<ClientPostLogoutRedirectUri>();
        public DbSet<ClientScope> ClientScopes => Set<ClientScope>();
        public DbSet<ClientSecret> ClientSecrets => Set<ClientSecret>();
        public DbSet<ClientClaim> ClientClaims => Set<ClientClaim>();
        public DbSet<ClientIdPRestriction> ClientIdPRestrictions => Set<ClientIdPRestriction>();
        public DbSet<ClientCorsOrigin> ClientCorsOrigins => Set<ClientCorsOrigin>();
        public DbSet<ClientProperty> ClientProperties => Set<ClientProperty>();
        public DbSet<PersistedGrant> PersistedGrants => Set<PersistedGrant>();
        public DbSet<DeviceFlowCodes> DeviceFlowCodes => Set<DeviceFlowCodes>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigureIdentityServer();
        }
    }

    private sealed class TestDbContextProvider(IIdentityServerDbContext context)
        : IDbContextProvider<IIdentityServerDbContext>
    {
        public IIdentityServerDbContext GetDbContext() => context;
        public Task<IIdentityServerDbContext> GetDbContextAsync() => Task.FromResult(context);
    }


    private static PersistedGrant BuildPersistedGrant(string sessionId, string subjectId, DateTime creationTime, DateTime? consumedTime = null)
    {
        var grant = (PersistedGrant)Activator.CreateInstance(typeof(PersistedGrant), nonPublic: true);
        grant.Key = Guid.NewGuid().ToString("N");
        grant.Type = "refresh_token";
        grant.ClientId = "client";
        grant.SessionId = sessionId;
        grant.SubjectId = subjectId;
        grant.CreationTime = creationTime;
        grant.ConsumedTime = consumedTime;
        grant.Data = "{}";
        EntityHelper.TrySetId(grant, () => Guid.NewGuid());
        return grant;
    }

    private static void InitializeRepositoryTenant(PersistentGrantRepository repository)
    {
        var currentTenant = new TestCurrentTenant();
        var lazyServiceProvider = Substitute.For<IAbpLazyServiceProvider>();
        lazyServiceProvider.LazyGetRequiredService<Volo.Abp.MultiTenancy.ICurrentTenant>()
            .Returns(currentTenant);

        var lazyProp = repository.GetType().GetProperty("LazyServiceProvider", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (lazyProp != null && lazyProp.CanWrite)
        {
            lazyProp.SetValue(repository, lazyServiceProvider);
            return;
        }

        var lazyField = repository.GetType().GetField("_lazyServiceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?? repository.GetType().GetField("lazyServiceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        lazyField?.SetValue(repository, lazyServiceProvider);
    }

    private SessionManager BuildSessionManager(IEnumerable<PersistedGrant> grants = null, IdentityUser user = null)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestIdentityServerDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TestIdentityServerDbContext(options);
        context.Database.EnsureCreated();
        if (grants != null)
        {
            context.Set<PersistedGrant>().AddRange(grants);
            context.SaveChanges();
        }

        var dbContextProvider = new TestDbContextProvider(context);
        var repository = new PersistentGrantRepository(dbContextProvider);
        InitializeRepositoryTenant(repository);

        var userManager = new TestIdentityUserManager();
        if (user != null)
        {
            userManager.AddUser(user);
        }

        var identitySecurityLogManager = BuildIdentitySecurityLogManager(userManager, user);

        var objectMapper = Substitute.For<IObjectMapper>();
        objectMapper.Map<PersistedGrant, UserSessionEto>(Arg.Any<PersistedGrant>())
            .Returns(callInfo =>
            {
                var grant = callInfo.Arg<PersistedGrant>();
                return new UserSessionEto
                {
                    Id = grant.SessionId,
                    UserId = grant.SubjectId,
                    SignInDate = grant.CreationTime,
                    Expiration = grant.Expiration
                };
            });

        return new SessionManager(
            CreateMockLogger<SessionManager>(),
            repository,
            identitySecurityLogManager,
            objectMapper,
            userManager,
            new HttpContextAccessor { HttpContext = new DefaultHttpContext() });
    }

    private SessionEventService BuildService(
        SessionManager sessionManager,
        IResponseContext responseContext,
        IDistributedEventBus eventBus,
        IUnitOfWorkManager unitOfWorkManager)
    {
        var logger = CreateMockLogger<SessionEventService>();
        return new SessionEventService(
            logger,
            responseContext,
            sessionManager,
            eventBus,
            unitOfWorkManager);
    }

    [Fact]
    public async Task HandleEventAsync_GetSessionById_ValidId_ReturnsSession()
    {
        var sessionId = "session-123";
        var userId = Guid.NewGuid();
        var grants = new[] { BuildPersistedGrant(sessionId, userId.ToString(), DateTime.UtcNow) };
        var sessionManager = BuildSessionManager(grants);

        var responseContext = Substitute.For<IResponseContext>();
        var eventBus = CreateMockEventBus();
        var unitOfWorkManager = CreateMockUnitOfWorkManager();

        var service = BuildService(sessionManager, responseContext, eventBus, unitOfWorkManager);
        var eventData = new GetSessionByIdRequestMsg { SessionId = sessionId };

        await service.HandleEventAsync(eventData);

        await responseContext.Received().RespondAsync(Arg.Any<SessionResponseMsg>());
    }

    [Fact]
    public async Task HandleEventAsync_GetSessionById_InvalidId_ThrowsException()
    {
        var sessionManager = BuildSessionManager(); // Empty database

        var responseContext = Substitute.For<IResponseContext>();
        var eventBus = CreateMockEventBus();
        var unitOfWorkManager = CreateMockUnitOfWorkManager();

        var service = BuildService(sessionManager, responseContext, eventBus, unitOfWorkManager);
        var eventData = new GetSessionByIdRequestMsg { SessionId = "invalid" };

        await Assert.ThrowsAnyAsync<Exception>(() => service.HandleEventAsync(eventData));
    }

    [Fact]
    public async Task HandleEventAsync_GetSessionsForUser_ReturnsSessions()
    {
        var userId = Guid.NewGuid();
        var grants = new[]
        {
            BuildPersistedGrant("session-1", userId.ToString(), DateTime.UtcNow),
            BuildPersistedGrant("session-2", userId.ToString(), DateTime.UtcNow)
        };
        var sessionManager = BuildSessionManager(grants);

        var responseContext = Substitute.For<IResponseContext>();
        var eventBus = CreateMockEventBus();
        var unitOfWorkManager = CreateMockUnitOfWorkManager();

        var service = BuildService(sessionManager, responseContext, eventBus, unitOfWorkManager);
        var eventData = new GetSessionsForUserRequestMsg { UserId = userId.ToString() };

        await service.HandleEventAsync(eventData);

        await responseContext.Received().RespondAsync(Arg.Is<SessionsResponseMsg>(m => m.Sessions.Count == 2));
    }

    [Fact]
    public async Task HandleEventAsync_RevokeSessionById_RevokesSession()
    {
        var sessionId = "session-123";
        var userId = Guid.NewGuid();
        var user = new IdentityUser(userId, "user", "user@example.com");
        var grants = new[] { BuildPersistedGrant(sessionId, userId.ToString(), DateTime.UtcNow) };
        var sessionManager = BuildSessionManager(grants, user);

        var responseContext = Substitute.For<IResponseContext>();
        var eventBus = Substitute.For<IDistributedEventBus>();
        var unitOfWorkManager = CreateMockUnitOfWorkManager();

        var service = BuildService(sessionManager, responseContext, eventBus, unitOfWorkManager);
        var eventData = new RevokeSessionByIdRequestMsg { SessionId = sessionId };

        await service.HandleEventAsync(eventData);

        await eventBus.Received().PublishAsync(Arg.Is<UserSessionsRevokedMsg>(m => m.SessionId == sessionId && m.UserId == userId));
    }

    [Fact]
    public async Task HandleEventAsync_RevokeSessionsForUser_RevokesAllSessions()
    {
        var userId = Guid.NewGuid();
        var user = new IdentityUser(userId, "user", "user@example.com");
        var grants = new[]
        {
            BuildPersistedGrant("session-1", userId.ToString(), DateTime.UtcNow),
            BuildPersistedGrant("session-2", userId.ToString(), DateTime.UtcNow)
        };
        var sessionManager = BuildSessionManager(grants, user);

        var responseContext = Substitute.For<IResponseContext>();
        var eventBus = Substitute.For<IDistributedEventBus>();
        var unitOfWorkManager = CreateMockUnitOfWorkManager();

        var service = BuildService(sessionManager, responseContext, eventBus, unitOfWorkManager);
        var eventData = new RevokeSessionsForUserRequestMsg { UserId = userId.ToString() };

        await service.HandleEventAsync(eventData);

        await eventBus.Received().PublishAsync(Arg.Is<UserSessionsRevokedMsg>(m => m.UserId == userId));
    }
}
