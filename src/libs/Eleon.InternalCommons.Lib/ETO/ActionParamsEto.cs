namespace Messaging.Module.ETO
{
  public class ActionParamsEto
  {
    public List<string> SourceDataSources { get; set; }
    public List<string> DestinationDataSources { get; set; }
    public Dictionary<string, string> Params { get; set; }
  }
}
