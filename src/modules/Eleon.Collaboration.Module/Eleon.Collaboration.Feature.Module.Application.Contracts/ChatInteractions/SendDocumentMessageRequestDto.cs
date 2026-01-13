using System;

namespace VPortal.Collaboration.Feature.Module.ChatInteractions
{
  public class SendDocumentMessageRequestDto
  {
    public Guid ChatId { get; set; }
    public string Filename { get; set; }
    public string DocumentBase64 { get; set; }
  }
}
