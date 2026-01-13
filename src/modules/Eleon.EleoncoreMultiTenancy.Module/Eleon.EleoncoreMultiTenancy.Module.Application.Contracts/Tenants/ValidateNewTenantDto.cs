namespace VPortal.TenantManagement.Module.Tenants;

public class ValidateNewTenantDto
{
  public bool IsNeedCreateDb { get; set; }
  public string NewDatabaseName { get; set; }
  public string NewUserName { get; set; }
  public bool IsNeedCreateSubDomain { get; set; }
  public string SubDomain { get; set; }
}
