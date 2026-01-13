using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleCollector.Accounting.Module.Accounting.Module.Domain.Shared.Constants;
public static class AccountingBackgroundJobTypes
{
  public const string ActivateAccountTenant = nameof(ActivateAccountTenant);
  public const string CreateTenantFromAccount = nameof(CreateTenantFromAccount);
  public const string ResetAdminPasswordAccountTenant = nameof(ResetAdminPasswordAccountTenant);
  public const string CancelAccountTenant = nameof(CancelAccountTenant);
  public const string SuspendAccountTenant = nameof(SuspendAccountTenant);
}
