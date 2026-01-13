using System;

namespace VPortal.Accounting.Module.Invoices
{
  public class InvoiceRowDto
  {
    public Guid Id { get; set; }
    public Guid InvoiceEntityId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public decimal RowTotal { get; set; }
  }
}
