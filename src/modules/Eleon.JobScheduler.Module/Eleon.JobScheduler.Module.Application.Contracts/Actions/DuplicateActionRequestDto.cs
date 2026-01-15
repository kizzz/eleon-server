

using Common.Module.Constants;

namespace VPortal.JobScheduler.Module.Actions
{
  public class DuplicateActionRequestDto
  {
    public Guid Id { get; set; }
    public int Count { get; set; }
    public string FieldToModify { get; set; }
  }
}
