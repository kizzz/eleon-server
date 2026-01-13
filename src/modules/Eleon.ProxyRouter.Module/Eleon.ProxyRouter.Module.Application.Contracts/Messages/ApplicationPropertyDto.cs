namespace ProxyRouter.Minimal.HttpApi.Models.Messages
{
  public class ApplicationPropertyDto
  {
    public required string Key { get; set; }
    public required string Value { get; set; }
    public required string Type { get; set; }
    public required string Level { get; set; }
  }
}
