using Common.Module.Constants;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class CreateTenantFromAccountMsg : VportalEvent
  {
    public string ObjectType { get; set; }
    public string DocumentId { get; set; }

    public new string TenantName { get; set; }
    public string AdminEmail { get; set; }
    public string AdminPassword { get; set; }
    public bool CreateDatabase { get; set; }
    public bool IsRoot { get; set; }
    public string NewDatabaseName { get; set; }
    public string NewUserName { get; set; }
    public string NewUserPassword { get; set; }
    public string DefaultConnectionString { get; set; }
    public Guid? AccountTenantId { get; set; }


  }
}
