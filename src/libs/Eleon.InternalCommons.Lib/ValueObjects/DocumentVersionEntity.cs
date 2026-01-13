namespace VPortal.Infrastructure.Module.Entities
{
  public class DocumentVersionEntity
  {
    public string Version { get; set; }
    public string TransactionId { get; set; }
    public string AppendToTransactionId { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string CreatedByUserName { get; set; }
  }
}
