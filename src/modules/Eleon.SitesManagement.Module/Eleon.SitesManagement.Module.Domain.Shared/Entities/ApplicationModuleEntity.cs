using Common.Module.Constants;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.SitesManagement.Module.Entities
{
  public class ApplicationModuleEntity : FullAuditedEntity<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public Guid ClientApplicationEntityId { get; set; }
    public string Url { get; set; }
    public string Name
    {
      get
      {
        return $"{PluginName}_{Expose.Replace("./", "")}";
      }
    }
    public string PluginName { get; set; }
    public Guid? ParentId { get; set; }
    public int OrderIndex { get; set; } = 0;
    public string Expose { get; set; }
    public UiModuleLoadLevel LoadLevel { get; set; }
    public List<ApplicationPropertyEntity> Properties { get; set; }

    public ApplicationModuleEntity(Guid id)
    {
      Id = id;
    }

    protected ApplicationModuleEntity() { }
  }
}


