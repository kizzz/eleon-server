using System;
using Volo.Abp.Domain.Repositories;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.Repositories
{
  public interface IClientApplicationRepository : IBasicRepository<ApplicationEntity, Guid>
  {
  }
}


