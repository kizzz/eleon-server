using Common.Module.Constants;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VPortal.ExternalLink.Module.Entities;

namespace VPortal.ExternalLink.Module.Repositories
{
  public interface IExternalLinkRepository : IBasicRepository<ExternalLinkEntity, Guid>
  {
    public Task<ExternalLinkEntity> GetAsync(string linkCode);
    public Task<List<ExternalLinkEntity>> GetByPrivateParamsAndDocTypeAsync(string privateParams, string documentType);
  }
}
