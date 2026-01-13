using System;
using Volo.Abp.Domain.Repositories;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module.Repositories
{
  public interface IBillingInformationRepository : IBasicRepository<BillingInformationEntity, Guid>
  {
  }
}
