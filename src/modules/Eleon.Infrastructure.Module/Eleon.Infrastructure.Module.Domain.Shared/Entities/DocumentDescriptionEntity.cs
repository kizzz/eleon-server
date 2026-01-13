using Common.Module.Constants;
using System;

namespace VPortal.Infrastructure.Module.Entities
{
  public class DocumentDescriptionEntity
  {
    public Guid Id { get; set; }
    public string Type { get; set; }
    public DocumentVersionEntity Version { get; set; }
  }
}
