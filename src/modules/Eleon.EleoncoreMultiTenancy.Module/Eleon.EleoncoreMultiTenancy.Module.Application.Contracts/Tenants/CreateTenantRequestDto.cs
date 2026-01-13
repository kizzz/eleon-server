namespace VPortal.TenantManagement.Module.Tenants
{
  public class CreateTenantRequestDto
  {
    public string TenantName { get; set; }
    public string AdminEmail { get; set; }
    public string AdminPassword { get; set; }
    public bool CreateDatabase { get; set; }
    public bool IsRoot { get; set; }
    public string NewDatabaseName { get; set; }
    public string NewUserName { get; set; }
    public string NewUserPassword { get; set; }
    public string DefaultConnectionString { get; set; }
  }
}
