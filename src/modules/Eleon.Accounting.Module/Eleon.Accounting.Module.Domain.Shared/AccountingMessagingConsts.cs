namespace VPortal.Accounting.Module
{
  public class AccountingMessagingConsts
  {
    public const string AccountingLifecycleCompleted = "AccountingLifecycleCompleted";
    public const string AckAccountingBackgroundJobCompletion = "AckAccountingBackgroundJobCompletion";
    public const string BackgroundJobCompleted = "AccountingBackgroundJobCompleted";
    public const string CreateAccounting = "CreateAccounting";
    public const string CreateTenant = "CreateTenant";

    public const string CreateTenantBackgroundJobCompleted = "CreateTenantBackgroundJobCompleted";
    public const string CancelAccountTenantBackgroundJobCompleted = "CancelAccountTenantBackgroundJobCompleted";
    public const string ActivateAccountTenantBackgroundJobCompleted = "ActivateAccountTenantBackgroundJobCompleted";
    public const string SuspendAccountTenantBackgroundJobCompleted = "SuspendAccountTenantBackgroundJobCompleted";
    public const string ResetTenantAdminPasswordBackgroundJobCompleted = "ResetTenantAdminPasswordBackgroundJobCompleted";

    public const string CancelAccountCompleted = "CancelAccountCompleted";
    public const string CancelAccountTenant = "CancelAccountTenant";
    public const string SuspendAccountTenant = "SuspendAccountTenant";
    public const string ResetTenantAdminPassword = "ResetTenantAdminPassword";
    public const string ActivateAccountTenant = "ActivateAccountTenant";
    public const string ResendMsg = "ResendMsg";
  }
}
