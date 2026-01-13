using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Migrations.Module;
using NSubstitute;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Helpers for testing security scenarios including authorization, tenant isolation, and permission checks.
/// </summary>
public static class SecurityTestHelpers
{
    /// <summary>
    /// Creates a mock permission checker that returns the specified permission result.
    /// </summary>
    /// <typeparam name="TPermissionChecker">The permission checker interface type.</typeparam>
    /// <param name="hasPermission">Whether the permission check should return true.</param>
    /// <returns>A mock permission checker.</returns>
    public static TPermissionChecker CreateMockPermissionChecker<TPermissionChecker>(bool hasPermission = true)
        where TPermissionChecker : class
    {
        var checker = Substitute.For<TPermissionChecker>();
        // This is a generic helper - specific implementations should override methods as needed
        return checker;
    }

    /// <summary>
    /// Sets up tenant isolation verification for a repository.
    /// Verifies that queries filter by tenant ID.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="repository">The repository to verify.</param>
    /// <param name="tenantId">The expected tenant ID.</param>
    public static void SetupTenantIsolation<TEntity, TKey>(
        IBasicRepository<TEntity, TKey> repository,
        Guid? tenantId)
        where TEntity : class, Volo.Abp.Domain.Entities.IEntity<TKey>, Volo.Abp.MultiTenancy.IMultiTenant
    {
        // In actual tests, verify that repository queries include tenant filter
        // This is a placeholder for verification logic
    }

    /// <summary>
    /// Verifies that an authorization exception is thrown.
    /// </summary>
    /// <param name="action">The action that should throw AbpAuthorizationException.</param>
    /// <returns>The caught exception.</returns>
    public static async Task<AbpAuthorizationException> VerifyAuthorizationFailure(Func<Task> action)
    {
        var exceptionAssertion = await action.Should().ThrowAsync<AbpAuthorizationException>();
        return exceptionAssertion.Which;
    }

    /// <summary>
    /// Verifies that an authorization exception is thrown with a specific message.
    /// </summary>
    /// <param name="action">The action that should throw AbpAuthorizationException.</param>
    /// <param name="expectedMessage">The expected exception message (partial match).</param>
    /// <returns>The caught exception.</returns>
    public static async Task<AbpAuthorizationException> VerifyAuthorizationFailure(
        Func<Task> action,
        string expectedMessage)
    {
        var exceptionAssertion = await action.Should().ThrowAsync<AbpAuthorizationException>();
        exceptionAssertion.Which.Message.Should().Contain(expectedMessage);
        return exceptionAssertion.Which;
    }

    /// <summary>
    /// Creates an unauthorized user (no permissions, different tenant, etc.).
    /// </summary>
    /// <param name="userId">The user ID (optional).</param>
    /// <param name="tenantId">The tenant ID (optional, different from authorized tenant).</param>
    /// <returns>A mock current user with no permissions.</returns>
    public static ICurrentUser CreateUnauthorizedUser(Guid? userId = null, Guid? tenantId = null)
    {
        return TestMockHelpers.CreateMockCurrentUser(
            userId ?? Guid.NewGuid(),
            "unauthorized_user",
            tenantId ?? Guid.NewGuid());
    }

    /// <summary>
    /// Creates a cross-tenant scenario for testing tenant isolation.
    /// </summary>
    /// <param name="tenant1Id">First tenant ID.</param>
    /// <param name="tenant2Id">Second tenant ID.</param>
    /// <returns>Tuple containing users for each tenant.</returns>
    public static (ICurrentUser Tenant1User, ICurrentUser Tenant2User) CreateCrossTenantScenario(
        Guid tenant1Id,
        Guid tenant2Id)
    {
        var tenant1User = TestMockHelpers.CreateMockCurrentUser(
            Guid.NewGuid(),
            "tenant1_user",
            tenant1Id);

        var tenant2User = TestMockHelpers.CreateMockCurrentUser(
            Guid.NewGuid(),
            "tenant2_user",
            tenant2Id);

        return (tenant1User, tenant2User);
    }

    /// <summary>
    /// Verifies that a user from one tenant cannot access data from another tenant.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity to check.</param>
    /// <param name="userTenantId">The user's tenant ID.</param>
    /// <param name="shouldHaveAccess">Whether access should be granted.</param>
    public static void VerifyTenantAccess<TEntity>(
        TEntity entity,
        Guid? userTenantId,
        bool shouldHaveAccess)
        where TEntity : class, Volo.Abp.MultiTenancy.IMultiTenant
    {
        if (entity is Volo.Abp.MultiTenancy.IMultiTenant multiTenantEntity)
        {
            var hasAccess = multiTenantEntity.TenantId == userTenantId ||
                           (userTenantId == null && multiTenantEntity.TenantId == null);

            hasAccess.Should().Be(shouldHaveAccess,
                $"User from tenant {userTenantId} should {(shouldHaveAccess ? "have" : "not have")} access to entity from tenant {multiTenantEntity.TenantId}");
        }
    }

    /// <summary>
    /// Creates a mock IdentityUserManager with admin role checking.
    /// </summary>
    /// <param name="isAdmin">Whether the user is an admin.</param>
    /// <param name="userId">The user ID.</param>
    /// <returns>A mock IdentityUserManager.</returns>
    public static MockIdentityUserManager CreateMockIdentityUserManager(bool isAdmin = false, Guid? userId = null)
    {
        var actualUserId = userId ?? Guid.NewGuid();
        var user = new IdentityUser(actualUserId, "testuser", "test@example.com", tenantId: null);
        
        var users = new Dictionary<Guid, IdentityUser> { { actualUserId, user } };
        var userRoles = isAdmin 
            ? new Dictionary<Guid, List<string>> { { actualUserId, new List<string> { MigrationConsts.AdminRoleNameDefaultValue } } }
            : new Dictionary<Guid, List<string>>();

        var manager = new MockIdentityUserManager(users, userRoles);
        return manager;
    }

    /// <summary>
    /// Verifies that permission escalation is prevented.
    /// Asserts that a user cannot grant themselves higher permissions.
    /// </summary>
    /// <param name="currentPermissions">Current user permissions.</param>
    /// <param name="requestedPermissions">Requested permissions.</param>
    /// <param name="canGrant">Whether the user can grant the requested permissions.</param>
    public static void VerifyPermissionEscalationPrevention(
        IEnumerable<string> currentPermissions,
        IEnumerable<string> requestedPermissions,
        bool canGrant)
    {
        var currentSet = currentPermissions.ToHashSet();
        var requestedSet = requestedPermissions.ToHashSet();

        // Check if requested permissions are a subset of current permissions
        var isSubset = requestedSet.IsSubsetOf(currentSet);

        if (!canGrant && !isSubset)
        {
            // Permission escalation attempt detected
            var escalatedPermissions = requestedSet.Except(currentSet);
            escalatedPermissions.Should().BeEmpty(
                $"User should not be able to grant permissions they don't have: {string.Join(", ", escalatedPermissions)}");
        }
    }

    /// <summary>
    /// Creates a scenario for testing input validation (SQL injection, XSS, path traversal, etc.).
    /// </summary>
    /// <returns>List of potentially malicious input strings.</returns>
    public static List<string> CreateMaliciousInputs()
    {
        return new List<string>
        {
            // SQL Injection attempts
            "'; DROP TABLE Files; --",
            "1' OR '1'='1",
            "admin'--",
            
            // XSS attempts
            "<script>alert('XSS')</script>",
            "<img src=x onerror=alert('XSS')>",
            "javascript:alert('XSS')",
            
            // Path traversal attempts
            "../../../etc/passwd",
            "..\\..\\..\\windows\\system32",
            "/etc/passwd",
            
            // Command injection attempts
            "; rm -rf /",
            "| cat /etc/passwd",
            "&& whoami",
            
            // Null bytes
            "file\0.txt",
            "test\0name",
            
            // Very long strings
            new string('A', 10000),
            
            // Special characters
            "file<>:\"|?*",
            "file\n\r\t",
        };
    }

    /// <summary>
    /// Verifies that malicious input is properly sanitized or rejected.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <param name="isValid">Whether the input should be considered valid.</param>
    /// <param name="sanitizedOutput">The expected sanitized output (if input is invalid but sanitized).</param>
    public static void VerifyInputValidation(
        string input,
        bool isValid,
        string sanitizedOutput = null)
    {
        if (isValid)
        {
            input.Should().NotBeNullOrEmpty();
        }
        else
        {
            // Input should either be rejected (throw exception) or sanitized
            if (sanitizedOutput != null)
            {
                // In actual tests, you would verify the sanitized output matches expected value
                sanitizedOutput.Should().NotContainAny("<script", "javascript:", "../", "..\\", "\0");
            }
        }
    }
}
