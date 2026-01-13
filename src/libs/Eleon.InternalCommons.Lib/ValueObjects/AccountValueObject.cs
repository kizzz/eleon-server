namespace Common.Module.ValueObjects
{
  public class AccountValueObject
  {
    public Guid? Id { get; set; }
    public Guid? TenantId { get; set; }
    public string DocEntry { get; set; }
    public string AccountName { get; set; }
    public string AccountNameEng { get; set; }
    public string AdminEmail { get; set; }
    public string AdminPassword { get; set; }
    public bool CreateDatabase { get; set; }
    public string NewDatabaseName { get; set; }
    public string NewUserName { get; set; }
    public string NewUserPassword { get; set; }
    public string DefaultConnectionString { get; set; }
    public Guid? AccountTenantId { get; set; }

    public AccountValueObject()
    {
    }
  }
}
