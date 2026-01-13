using System;
using System.Threading.Tasks;

namespace VPortal.Accounting.Module.BillingInformations
{
  public interface IBillingInformationAppService
  {
    Task<BillingInformationDto> GetBillingInfoDetailsById(Guid id);
  }
}
