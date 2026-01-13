using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Tenants;

public interface ITenantAppService
{
  Task<CommonTenantDto> GetCommonTenant(Guid tenantId);
  Task<List<CommonTenantDto>> GetCommonTenantListAsync();
  Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListAsync();
  Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListWithCurrentAsync();
}
