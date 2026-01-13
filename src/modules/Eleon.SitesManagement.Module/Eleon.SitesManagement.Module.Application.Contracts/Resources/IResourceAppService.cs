using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using VPortal.SitesManagement.Module.DomainServices;
using VPortal.SitesManagement.Module.Entities;
using VPortal.SitesManagement.Module.Microservices;

namespace VPortal.SitesManagement.Module.Resources
{
  public interface IResourceAppService : IApplicationService
  {
    public Task<EleoncoreModuleDto> GetAsync(Guid id);
    public Task<List<EleoncoreModuleDto>> GetAllAsync();
    public Task<EleoncoreModuleDto> CreateAsync(EleoncoreModuleDto input);
    public Task<EleoncoreModuleDto> UpdateAsync(EleoncoreModuleDto input);
    public Task DeleteAsync(Guid id);
  }
}


