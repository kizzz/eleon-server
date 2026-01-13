using Common.Module.Constants;
using System;

namespace VPortal.Accounting.Module.Invoices
{
  public class ReceiptDto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid InvoiceEntityId { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public string Transaction { get; set; }
  }
}
