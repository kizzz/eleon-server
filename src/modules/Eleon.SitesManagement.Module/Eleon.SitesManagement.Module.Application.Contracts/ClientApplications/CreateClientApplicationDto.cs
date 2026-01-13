using Common.Module.Constants;
using ModuleCollector.Commons.Module.Proxy.Constants;
using System;

namespace VPortal.SitesManagement.Module.ClientApplications
{

  public class CreateClientApplicationDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public bool IsAuthenticationRequired { get; set; }
    public string RequiredPolicy { get; set; }
    public ClientApplicationFrameworkType FrameworkType { get; set; }
    public ClientApplicationStyleType StyleType { get; set; }
    public ClientApplicationType ClientApplicationType { get; set; }
    public Guid? TenantId { get; set; }
    public string Path { get; set; }
    public string Icon { get; set; }
    public List<ClientApplicationPropertyDto>? Properties { get; set; }
    public Guid? ParentId { get; set; }
    public ApplicationType AppType { get; set; }
  }
}


