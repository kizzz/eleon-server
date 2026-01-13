using ProxyRouter.Minimal.HttpApi.Models.Constants;
using SharedCollector.deprecated.Common.Module.Constants;

namespace ProxyRouter.Minimal.HttpApi.Models.Messages
{
  public class ClientApplicationDto
  {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required string Source { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public required string HeadString { get; set; }
    public required string Icon { get; set; }
    public ErrorHandlingLevel ErrorHandlingLevel { get; set; }
    public ClientApplicationFrameworkType FrameworkType { get; set; }
    public ClientApplicationStyleType StyleType { get; set; }
    public ClientApplicationType ClientApplicationType { get; set; }
    public bool UseDedicatedDatabase { get; set; }
    public required List<ApplicationModuleDto> Modules { get; set; }
    public bool IsSystem { get; set; }
    public bool IsAuthenticationRequired { get; set; }
    public string RequiredPolicy { get; set; }
    public required List<ApplicationPropertyDto> Properties { get; set; }

    public ApplicationType AppType { get; set; }
    public Guid? ParentId { get; set; }
    public string Expose { get; set; }
    public string LoadLevel { get; set; }
    public int OrderIndex { get; set; }
  }
}
