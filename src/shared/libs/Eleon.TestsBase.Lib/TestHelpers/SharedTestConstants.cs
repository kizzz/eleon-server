using System;

namespace Eleon.TestsBase.Lib.TestHelpers;

/// <summary>
/// Common test constants shared across modules.
/// </summary>
public static class SharedTestConstants
{
    public static class TenantIds
    {
        public static readonly Guid? Host = null;
        public static readonly Guid Tenant1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Tenant2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
    }

    public static class Users
    {
        public const string TestUser = "testuser";
        public const string AdminUser = "admin";
        public const string SystemUser = "system";
    }

    public static class Dates
    {
        public static readonly DateTime UtcNow = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc);
        public static readonly DateTime PastDate = UtcNow.AddDays(-1);
        public static readonly DateTime FutureDate = UtcNow.AddDays(1);
        public static readonly DateTime FarFutureDate = UtcNow.AddDays(30);
    }
}
