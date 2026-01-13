using Common.Module.Constants;

namespace Messaging.Module.ETO
{
  public class TenantEto
  {
    public string Name { get; set; }
    public Guid Id { get; set; }
    public TenantStatus Status { get; set; }
    public string ParentId { get; set; }
    public bool IsRoot { get; set; }

    public List<TenantConnectionStringEto> ConnectionStrings { get; set; } = new List<TenantConnectionStringEto>();
  }

  public class TenantConnectionStringEto
  {
    public string Name { get; set; }
    public string Value { get; set; }
  }
}
