using System;

namespace VPortal.Infrastructure.Module.Countries
{
  public class CountryDto
  {
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
  }
}
