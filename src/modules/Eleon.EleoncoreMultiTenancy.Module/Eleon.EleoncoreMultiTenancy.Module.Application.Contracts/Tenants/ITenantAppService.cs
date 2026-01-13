using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.Tenants;

public interface ITenantAppService
{
  Task<CommonTenantDto> GetCommonTenant(Guid tenantId);
  Task<List<CommonTenantDto>> GetCommonTenantListAsync();
  Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListAsync();
  Task<List<CommonTenantExtendedDto>> GetCommonTenantExtendedListWithCurrentAsync();
  Task<TenantCreationResult> CreateCommonTenant(CreateTenantRequestDto request);
  Task RemoveCommonTenant(CommonTenantDto tenantDto);
  Task<string> CreateDatabase(CreateDatabaseDto createDatabaseDto);
}
