using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.TestBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
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
using Volo.Abp.Users;
using VPortal.Identity.Module.Entities;
using VPortal.Identity.Module.Sessions;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Eleonsoft.Tests.modules.Eleon.Auth.Module.tests.EntityFrameworkCore;

public class SessionManagerTests : DomainTestBase
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

    private static IdentitySecurityLogManager BuildSecurityLogManager(
        TestIdentityUserManager userManager,
        ISecurityLogManager securityLogManager,
        IdentityUser currentUser)
    {
        var principalAccessor = Substitute.For<ICurrentPrincipalAccessor>();
        principalAccessor.Change(Arg.Any<ClaimsPrincipal>()).Returns(Substitute.For<IDisposable>());

        var principalFactory = Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>();
        principalFactory.CreateAsync(Arg.Any<IdentityUser>()).Returns(Task.FromResult(new ClaimsPrincipal()));

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

    private (SessionManager manager, TestIdentityServerDbContext context, SecurityLogInfo lastLog) BuildManager(IEnumerable<PersistedGrant> grants, IdentityUser user)
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

        var capturedLog = new SecurityLogInfo();
        var securityLogManager = Substitute.For<ISecurityLogManager>();
        securityLogManager.SaveAsync(Arg.Do<Action<SecurityLogInfo>>(action =>
        {
            action(capturedLog);
        })).Returns(Task.CompletedTask);

        var identitySecurityLogManager = BuildSecurityLogManager(userManager, securityLogManager, user);

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

        var manager = new SessionManager(
            CreateMockLogger<SessionManager>(),
            repository,
            identitySecurityLogManager,
            objectMapper,
            userManager,
            new HttpContextAccessor { HttpContext = new DefaultHttpContext() });

        return (manager, context, capturedLog);
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

    [Fact]
    public async Task GetByIdAsync_UsesMaxConsumedTimeAsLastAccess()
    {
        var sessionId = "session-1";
        var now = DateTime.UtcNow;
        List<PersistedGrant> grants =
        [
            BuildPersistedGrant(sessionId, "user", now.AddMinutes(-10), now.AddMinutes(-5)),
            BuildPersistedGrant(sessionId, "user", now.AddMinutes(-7), now.AddMinutes(-1))
        ];

        var (manager, _, _) = BuildManager(grants, null);

        var session = await manager.GetByIdAsync(sessionId);

        Assert.Equal(sessionId, session.Id);
        Assert.Equal(now.AddMinutes(-1), session.LastAccessTime);
    }

    [Fact]
    public async Task GetAllAsync_DeduplicatesSessionsAndUsesSignInDate()
    {
        var now = DateTime.UtcNow;
        List<PersistedGrant> grants =
        [
            BuildPersistedGrant("s1", "user", now.AddMinutes(-30), now.AddMinutes(-20)),
            BuildPersistedGrant("s1", "user", now.AddMinutes(-25)),
            BuildPersistedGrant("s2", "user", now.AddMinutes(-15))
        ];

        var (manager, _, _) = BuildManager(grants, null);

        var sessions = await manager.GetAllAsync();

        Assert.Equal(2, sessions.Count);
        var session1 = sessions.Single(s => s.Id == "s1");
        var session2 = sessions.Single(s => s.Id == "s2");
        Assert.Equal(now.AddMinutes(-20), session1.LastAccessTime);
        Assert.Equal(session2.SignInDate, session2.LastAccessTime);
    }

    [Fact]
    public async Task RevokeAsync_MissingSession_Throws()
    {
        var (manager, _, _) = BuildManager(Array.Empty<PersistedGrant>(), null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => manager.RevokeAsync("missing"));
    }

    [Fact]
    public async Task RevokeAsync_LogsSecurityAction()
    {
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        List<PersistedGrant> grants =
        [
            BuildPersistedGrant("s1", user.Id.ToString(), DateTime.UtcNow)
        ];

        var (manager, _, lastLog) = BuildManager(grants, user);

        await manager.RevokeAsync("s1");

        Assert.NotNull(lastLog);
        Assert.Equal("RevokeSession", lastLog.Action);
        Assert.Equal(user.UserName, lastLog.UserName);
    }

    [Fact]
    public async Task RevokeAllAsync_LogsSecurityAction()
    {
        var user = new IdentityUser(Guid.NewGuid(), "user", "user@example.com");
        List<PersistedGrant> grants =
        [
            BuildPersistedGrant("s1", user.Id.ToString(), DateTime.UtcNow),
            BuildPersistedGrant("s2", user.Id.ToString(), DateTime.UtcNow)
        ];

        var (manager, _, lastLog) = BuildManager(grants, user);

        await manager.RevokeAllAsync(user.Id.ToString());

        Assert.NotNull(lastLog);
        Assert.Equal("RevokeAllSessions", lastLog.Action);
        Assert.Equal(user.UserName, lastLog.UserName);
    }
}
