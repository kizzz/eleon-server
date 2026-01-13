using Common.Module.Constants;
using System;

namespace VPortal.Accounting.Module.BillingInformations
{
  public class BillingInformationDto
  {
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string CompanyCID { get; set; }
    public string BillingAddressLine1 { get; set; }
    public string BillingAddressLine2 { get; set; }
    public string City { get; set; }
    public string StateOrProvince { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string ContactPersonName { get; set; }
    public string ContactPersonEmail { get; set; }
    public string ContactPersonTelephone { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
  }
}
