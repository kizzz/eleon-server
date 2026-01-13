using System;
using System.Collections.Generic;

namespace VPortal.Accounting.Module.Invoices
{
  public class InvoiceDto
  {
    public Guid Id { get; set; }
    public Guid AccountEntityId { get; set; }
    public string CustomerName { get; set; }
    public string CompanyCID { get; set; }
    public string BillingAddress { get; set; }
    public string BillingCity { get; set; }
    public string BillingState { get; set; }
    public string BillingCountry { get; set; }
    public string BillingPostalCode { get; set; }
    public string Currency { get; set; }
    public decimal Total { get; set; }
    public List<InvoiceRowDto> InvoiceRows { get; set; }
    public ReceiptDto Receipt { get; set; }
  }
}
