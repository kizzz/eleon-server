using System;
using Volo.Abp.Domain.Repositories;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.Microservices
{
  public interface IModulesRepository : IBasicRepository<ModuleEntity, Guid>
  {
  }
}


