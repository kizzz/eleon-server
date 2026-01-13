using Common.Module.Constants;

namespace VPortal.SitesManagement.Module.ClientApplications
{

  public class UpdateClientApplicationDto
  {
    public string Name { get; set; }
    public string Path { get; set; }
    public string Source { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsAuthenticationRequired { get; set; }
    public string RequiredPolicy { get; set; }
    public ClientApplicationFrameworkType FrameworkType { get; set; }
    public ClientApplicationStyleType StyleType { get; set; }
    public ClientApplicationType ClientApplicationType { get; set; }
    public string Icon { get; set; }
    public List<ClientApplicationPropertyDto>? Properties { get; set; }
    public bool IsDefault { get; set; }
  }
}


