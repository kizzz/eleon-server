namespace ProxyRouter.Minimal.HttpApi.Models.Messages
{
  public class ApplicationModuleDto
  {
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Name { get; set; }
    public required string PluginName { get; set; }
    public Guid? ParentId { get; set; }
    public required string LoadLevel { get; set; }
    public int OrderIndex { get; set; }
    public required string Expose { get; set; }
    public required List<ApplicationPropertyDto> Properties { get; set; }
    public Guid ClientApplicationEntityId { get; set; }
  }
}
