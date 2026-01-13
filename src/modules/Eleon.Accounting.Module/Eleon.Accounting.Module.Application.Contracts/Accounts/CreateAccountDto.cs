using System;

namespace VPortal.Accounting.Module.Accounts
{
  public class CreateAccountDto
  {
    public string DataSourceUid { get; set; }

    public string DataSourceName { get; set; }

    public string CompanyUid { get; set; }

    public string CompanyName { get; set; }

    public Guid OrganizationUnitId { get; set; }

    public string OrganizationUnitName { get; set; }
    public Guid OwnerId { get; set; }
  }
}
