using EleonsoftModuleCollector.Infrastructure.Module.Infrastructure.Module.Domain.Permissions.Constants;
using Logging.Module;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Overrides;
using NUglify.JavaScript.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using VPortal.TenantManagement.Module.PermissionGroups;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class PermissionGroupsDomainService : DomainService
  {
    private readonly CustomPermissionDomainService customPermissionDomainService;
    private readonly CustomPermissionDefinitionManager customPermissionDefinitionManager;
    private readonly IVportalLogger<PermissionGroupsDomainService> logger;
    private readonly IPermissionGroupDefinitionRecordRepository _permissionGroupDefinitionRecordRepository;

    public PermissionGroupsDomainService(
        CustomPermissionDomainService customPermissionDomainService,
        CustomPermissionDefinitionManager customPermissionDefinitionManager,
        IVportalLogger<PermissionGroupsDomainService> logger,
        IPermissionGroupDefinitionRecordRepository permissionGroupDefinitionRecordRepository)
    {
      this.customPermissionDomainService = customPermissionDomainService;
      this.customPermissionDefinitionManager = customPermissionDefinitionManager;
      this.logger = logger;
      this._permissionGroupDefinitionRecordRepository = permissionGroupDefinitionRecordRepository;
    }

    public async Task<List<PermissionGroupCategory>> GetPermissionGroupCategories()
    {
      List<PermissionGroupCategory> result = new List<PermissionGroupCategory>();

      try
      {
        var permissionGroups = await _permissionGroupDefinitionRecordRepository.GetListAsync(); // customPermissionDefinitionManager.GetGroupsAsync();
                                                                                                //var permissions = await customPermissionDomainService.GetPermissionsDynamic();

        //result = permissionGroups
        //    .Select(pg => new PermissionGroupCategory(
        //        pg.Name,
        //        null, // No children categories
        //        permissions.Where(p => p.GroupName == pg.Name).Select(p => p.Name).ToList() // List of permission names within the group
        //    ))
        //    .ToList();
        result = permissionGroups.GroupBy(
            (key) => (string)key.GetProperty(PermissionConstants.SourceIdPropertyName, string.Empty)) // "CategoryName", "TenantManagement::Uncategorized"
            .Select(group =>
            {
              return
                          new PermissionGroupCategory(
                          group.Key,
                          group.Select(g => new PermissionGroup(
                              g.Name,
                              g.GetProperty("Dynamic", false),
                              g.GetProperty("Order", 0))
                          ).ToList());
            })
            .OrderBy(t => t.Name)
            .ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}
