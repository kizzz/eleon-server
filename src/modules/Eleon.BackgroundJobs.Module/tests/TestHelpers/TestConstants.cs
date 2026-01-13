using System;
using Eleon.TestsBase.Lib.TestHelpers;

namespace BackgroundJobs.Module.TestHelpers;

public static class TestConstants
{
    public static class TenantIds
    {
        public static readonly Guid? Host = SharedTestConstants.TenantIds.Host;
        public static readonly Guid Tenant1 = SharedTestConstants.TenantIds.Tenant1;
        public static readonly Guid Tenant2 = SharedTestConstants.TenantIds.Tenant2;
    }

    public static class JobIds
    {
        public static readonly Guid Job1 = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        public static readonly Guid Job2 = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        public static readonly Guid Job3 = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
    }

    public static class ExecutionIds
    {
        public static readonly Guid Execution1 = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD");
        public static readonly Guid Execution2 = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        public static readonly Guid Execution3 = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
    }

    public static class JobTypes
    {
        public const string TestJob = "TestJob";
        public const string SystemJob = "SystemJob";
        public const string ImportJob = "ImportJob";
        public const string ExportJob = "ExportJob";
    }

    public static class Users
    {
        public const string TestUser = SharedTestConstants.Users.TestUser;
        public const string AdminUser = SharedTestConstants.Users.AdminUser;
        public const string SystemUser = SharedTestConstants.Users.SystemUser;
        public const string ApiKey = "test-api-key";
    }

    public static class Dates
    {
        public static readonly DateTime UtcNow = SharedTestConstants.Dates.UtcNow;
        public static readonly DateTime PastDate = SharedTestConstants.Dates.PastDate;
        public static readonly DateTime FutureDate = SharedTestConstants.Dates.FutureDate;
        public static readonly DateTime FarFutureDate = SharedTestConstants.Dates.FarFutureDate;
    }

    public static class Messages
    {
        public const string TestMessage = "Test message";
        public const string ErrorMessage = "Error occurred";
        public const string SuccessMessage = "Operation completed successfully";
        public const string WarningMessage = "Warning: Check configuration";
    }
}
