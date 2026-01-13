using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.MultiTenancy;

namespace VPortal.SitesManagement.Module.CustomFeatures
{
  public class CustomFeatureDto
  {
    public Guid Id { get; set; }

    public string GroupName { get; set; }

    public string Name { get; set; }

    public string ParentName { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string DefaultValue { get; set; }

    public bool IsVisibleToClients { get; set; }

    public bool IsAvailableToHost { get; set; }

    public string AllowedProviders { get; set; }
    public string ValueType { get; set; }

    public bool IsDynamic { get; set; }
    public string SourceId { get; set; }
  }
}


