using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;

namespace VPortal.Auditor.Module.Remote
{
  public interface IAbpAuditStoreRemoteAppService : IApplicationService
  {
    Task<bool> SaveAuditLogInfo(AuditLogInfo auditInfo);
  }
}
