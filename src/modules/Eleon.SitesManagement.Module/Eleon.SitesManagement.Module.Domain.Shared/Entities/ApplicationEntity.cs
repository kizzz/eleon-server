using Common.Module.Constants;
using ModuleCollector.Commons.Module.Proxy.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace VPortal.SitesManagement.Module.Entities
{
  public class ApplicationEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public ApplicationEntity(Guid id)
    {
      Id = id;
      Modules = new List<ApplicationModuleEntity>();
      Properties = new List<ApplicationPropertyEntity>();
    }

    protected ApplicationEntity()
    {

      Modules = new List<ApplicationModuleEntity>();
      Properties = new List<ApplicationPropertyEntity>();
    }

    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }                                                          // AppModule: URL
    public string Path { get; set; }
    public bool IsEnabled { get; set; }
    public ClientApplicationFrameworkType FrameworkType { get; set; }
    public ClientApplicationStyleType StyleType { get; set; }
    public ClientApplicationType ClientApplicationType { get; set; }
    public ErrorHandlingLevel ErrorHandlingLevel { get; set; } = ErrorHandlingLevel.Debug;
    public List<ApplicationModuleEntity> Modules { get; set; }
    public string Icon { get; set; }
    public bool IsDefault { get; set; }
    public string HeadString { get; set; }
    public bool UseDedicatedDatabase { get; set; }
    public bool IsAuthenticationRequired { get; set; }
    public string RequiredPolicy { get; set; }
    public List<ApplicationPropertyEntity> Properties { get; set; }

    public ApplicationType AppType { get; set; }
    public Guid? ParentId { get; set; }

    // Expose + Name (Plugin Name) -> is key value for
    public string Expose { get; set; }                                                          // AppModule:Expose
    public UiModuleLoadLevel LoadLevel { get; set; }                                            // AppModule:LoadLevel
    public int OrderIndex { get; set; }                                                         // AppModule:OrderIndex

    [NotMapped]
    public bool IsSystem { get; set; } = false;
  }
}


