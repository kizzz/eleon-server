using Common.Module.Constants;
using Common.Module.Serialization;

namespace Messaging.Module.Messages
{
  public class GetDocumentIdsByFilterGotMsg : VportalEvent
  {
    public List<string> Ids { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMsg { get; set; }
  }
}
