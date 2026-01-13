using System;

namespace VPortal.TenantManagement.Module.UserSettings
{
  public class ChangeOrganizationUnitDatasourceCompanySettingsDto
  {
    public Guid OrganizationUnitId { get; set; }
    public string DatasourceUid { get; set; }
    public string CompanyUid { get; set; }
  }
}
