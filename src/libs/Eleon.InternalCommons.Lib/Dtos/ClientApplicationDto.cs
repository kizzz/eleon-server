using Common.Module.Constants;
using ModuleCollector.Commons.Module.Proxy.Constants;

namespace VPortal.SitesManagement.Module.ClientApplications
{
  public class ClientApplicationDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Source { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public string HeadString { get; set; }
    public string Icon { get; set; }
    public ClientApplicationFrameworkType FrameworkType { get; set; }
    public ClientApplicationStyleType StyleType { get; set; }
    public ClientApplicationType ClientApplicationType { get; set; }
    public ErrorHandlingLevel ErrorHandlingLevel { get; set; }
    public bool UseDedicatedDatabase { get; set; }
    public bool IsSystem { get; set; }
    public bool IsAuthenticationRequired { get; set; }
    public string RequiredPolicy { get; set; }
    public List<ClientApplicationPropertyDto> Properties { get; set; }
    public ApplicationType AppType { get; set; }
    public Guid? ParentId { get; set; }

    public string Expose { get; set; }
    public string LoadLevel { get; set; }
    public int OrderIndex { get; set; }
  }
}


