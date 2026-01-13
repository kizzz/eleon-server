using System;
using Eleon.TestsBase.Lib.TestHelpers;

namespace JobScheduler.Module.TestHelpers;

public static class TestConstants
{
    public static class TenantIds
    {
        public static readonly Guid? Host = SharedTestConstants.TenantIds.Host;
        public static readonly Guid Tenant1 = SharedTestConstants.TenantIds.Tenant1;
        public static readonly Guid Tenant2 = SharedTestConstants.TenantIds.Tenant2;
    }

    public static class TaskIds
    {
        public static readonly Guid Task1 = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        public static readonly Guid Task2 = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        public static readonly Guid Task3 = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC");
    }

    public static class ActionIds
    {
        public static readonly Guid Action1 = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD");
        public static readonly Guid Action2 = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        public static readonly Guid Action3 = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
    }

    public static class TriggerIds
    {
        public static readonly Guid Trigger1 = Guid.Parse("10000000-0000-0000-0000-000000000001");
        public static readonly Guid Trigger2 = Guid.Parse("20000000-0000-0000-0000-000000000002");
    }

    public static class TaskExecutionIds
    {
        public static readonly Guid Execution1 = Guid.Parse("30000000-0000-0000-0000-000000000001");
        public static readonly Guid Execution2 = Guid.Parse("40000000-0000-0000-0000-000000000002");
    }

    public static class ActionExecutionIds
    {
        public static readonly Guid ActionExecution1 = Guid.Parse("50000000-0000-0000-0000-000000000001");
        public static readonly Guid ActionExecution2 = Guid.Parse("60000000-0000-0000-0000-000000000002");
    }

    public static class JobIds
    {
        public static readonly Guid Job1 = Guid.Parse("70000000-0000-0000-0000-000000000001");
        public static readonly Guid Job2 = Guid.Parse("80000000-0000-0000-0000-000000000002");
    }

    public static class TaskNames
    {
        public const string TestTask = "TestTask";
        public const string SystemTask = "SystemTask";
        public const string ImportTask = "ImportTask";
        public const string ExportTask = "ExportTask";
    }

    public static class ActionNames
    {
        public const string TestAction = "TestAction";
        public const string ProcessAction = "ProcessAction";
        public const string ValidateAction = "ValidateAction";
    }

    public static class TriggerNames
    {
        public const string DailyTrigger = "DailyTrigger";
        public const string WeeklyTrigger = "WeeklyTrigger";
        public const string MonthlyTrigger = "MonthlyTrigger";
    }

    public static class Users
    {
        public static readonly Guid User1 = Guid.Parse("90000000-0000-0000-0000-000000000001");
        public const string TestUser = SharedTestConstants.Users.TestUser;
        public const string AdminUser = SharedTestConstants.Users.AdminUser;
        public const string SystemUser = SharedTestConstants.Users.SystemUser;
    }

    public static class Dates
    {
        public static readonly DateTime UtcNow = SharedTestConstants.Dates.UtcNow;
        public static readonly DateTime PastDate = SharedTestConstants.Dates.PastDate;
        public static readonly DateTime FutureDate = SharedTestConstants.Dates.FutureDate;
        public static readonly DateTime FarFutureDate = SharedTestConstants.Dates.FarFutureDate;
    }
}
